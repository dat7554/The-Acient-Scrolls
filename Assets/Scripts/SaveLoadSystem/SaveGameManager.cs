using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InventorySystem.ScriptableObjects;
using SaveLoadSystem.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace SaveLoadSystem
{
    public class SaveGameManager : MonoBehaviour
    {
        public static SaveGameManager Instance { get; private set; }
     
        [Header("Save File Storage Config")]
        [SerializeField] private string saveDirectory;
        [SerializeField] private string fileName;
        
        public GameData CurrentGameData;
        public UnityAction<GameData> OnGameDataLoaded;

        public string CurrentProfileId { get; private set; }
        
        private bool _isSaving;
        private bool _isLoading;
        
        private IGameDataHandler _gameDataHandler;

        private void Awake()
        {
            if (Instance is null) 
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            var fullPath = Path.Combine(Application.persistentDataPath, saveDirectory);
            _gameDataHandler = new FileGameDataHandler(fullPath, fileName);
            CurrentProfileId = _gameDataHandler.GetRecentlyUpdatedProfileId();
            Debug.Log($"Loaded profile {CurrentProfileId}");
        }

        public void ChangeProfileId(string newProfileId)
        {
            this.CurrentProfileId = newProfileId;
        }

        public void ChangeToRecentlyUpdatedProfileId()
        {
            CurrentProfileId = _gameDataHandler.GetRecentlyUpdatedProfileId();
        }

        public void SaveGame()
        {
            if (_isSaving || _isLoading) return;
            _isSaving = true;

            try
            {
                CurrentGameData ??= new GameData();

                // Gather all IDataPersistence components
                var handlers = FindObjectsByType<MonoBehaviour>
                    (FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDataPersistence>().ToList();
                
                foreach (var handler in handlers)
                    handler.SaveData(CurrentGameData);

                CurrentGameData.dateTimeISO = DateTime.Now.ToString("O");
                _gameDataHandler.Save(CurrentGameData, CurrentProfileId);
            }
            finally
            {
                _isSaving = false;
            }
        }

        public void LoadGame()
        {
            if (_isSaving || _isLoading) return;
            _isLoading = true;

            try
            {
                CurrentGameData = _gameDataHandler.Load(CurrentProfileId);
                if (CurrentGameData != null)
                {
                    SpawnActiveItems(); // Spawn active items first (so IDs exist before handlers load)
                    OnGameDataLoaded?.Invoke(CurrentGameData);
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        public Dictionary<string, GameData> GetAllGameDataProfiles()
        {
            return _gameDataHandler.LoadAllGameDataProfiles();
        }
        
        // TODO: fix bug items duplicate
        private void SpawnActiveItems()
        {
            foreach (var (id, itemPickupData) in CurrentGameData.activeItems)
            {
                if (UniqueIdGenerator.IsRegistered(id)) continue;
                
                // TODO: move this to some manager
                if (itemPickupData.itemID == -1) return;
                var itemDatabase = Resources.Load<ItemDatabase>("Items/ItemDatabase");
                var itemData = itemDatabase.GetItemDataFromItemDatabase(itemPickupData.itemID);
                //
                
                var prefab = itemData.ItemPrefab;
                if (prefab is null)
                {
                    Debug.LogWarning($"No prefab found for item {itemData.name}");
                    continue;
                }

                var instance = GameManager.Instance.SpawnItem(prefab, itemPickupData.position, itemPickupData.rotation);

                // Apply saved ID so it matches what is in the save data
                var idGenerator = instance.GetComponent<UniqueIdGenerator>();
                idGenerator?.ForceSetID(id);
            }
        }
    }
}
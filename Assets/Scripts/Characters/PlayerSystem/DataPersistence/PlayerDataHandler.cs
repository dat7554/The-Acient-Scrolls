using InventorySystem.DataPersistence;
using InventorySystem.UI;
using SaveLoadSystem;
using UnityEngine;

namespace Characters.PlayerSystem.DataPersistence
{
    public class PlayerDataHandler : InventoryDataHandler
    {
        [SerializeField] private HotbarUI hotbarUI;
        
        private PlayerCharacter _playerCharacter;
        private PlayerCamera _playerCamera;
        private PlayerStats _playerStats;
        private PlayerInventoryHolder _playerInventoryHolder;

        private PlayerData _playerData;

        private void Awake()
        {
            _playerCharacter = GetComponentInChildren<PlayerCharacter>();
            _playerCamera = GetComponentInChildren<PlayerCamera>();
        
            _playerStats = GetComponent<PlayerStats>();
            _playerInventoryHolder = GetComponent<PlayerInventoryHolder>();
        }

        private void OnEnable()
        {
            SaveGameManager.Instance.OnGameDataLoaded += LoadData;
        }

        private void OnDestroy()
        {
            SaveGameManager.Instance.OnGameDataLoaded -= LoadData;
        }

        public override void SaveData(GameData gameData)
        {
            _playerData.position = _playerCharacter.transform.position;
            _playerData.characterRotation = _playerCharacter.transform.rotation;
            _playerData.cameraRotation = _playerCamera.EulerAngles;
        
            _playerData.currentHealth = _playerStats.CurrentHealth;
            _playerData.maxHealth = _playerStats.MaxHealth;
        
            _playerData.currentStamina = _playerStats.CurrentStamina;
            _playerData.maxStamina = _playerStats.MaxStamina;
        
            _playerData.hotbarInventoryData.container = _playerInventoryHolder.Container;
            _playerData.backpackInventoryData.container = _playerInventoryHolder.BackpackContainer;
        
            gameData.playerData = _playerData;
        }

        public override void LoadData(GameData dataToLoad)
        {
            var playerDataToLoad = dataToLoad.playerData;
        
            _playerCamera.LoadCameraRotation(playerDataToLoad.cameraRotation);
        
            _playerCharacter.transform.position = playerDataToLoad.position;
            _playerCharacter.transform.rotation = playerDataToLoad.characterRotation;
        
            _playerStats.LoadData
            (
                playerDataToLoad.currentHealth, 
                playerDataToLoad.maxHealth, 
                playerDataToLoad.currentStamina, 
                playerDataToLoad.maxStamina
            );
        
            if (playerDataToLoad.hotbarInventoryData.container is not null)
                _playerInventoryHolder.LoadContainer(playerDataToLoad.hotbarInventoryData.container);
        
            if (playerDataToLoad.backpackInventoryData.container is not null)
                _playerInventoryHolder.LoadBackpackContainer(playerDataToLoad.backpackInventoryData.container);
            
            // Refresh hotbar UI after loading container
            if (hotbarUI != null)
            {
                hotbarUI.RefreshUI();
            }
        
            Physics.SyncTransforms();
        }
    }
}
using System;
using InventorySystem.DataPersistence;
using SaveLoadSystem;

namespace InventorySystem.Chest
{
    public class ChestDataHandler : InventoryDataHandler
    {
        private string _id;
        private ChestInventoryHolder _chestInventoryHolder;
        
        private InventoryData _inventoryData;

        private void Awake()
        {
            _chestInventoryHolder = GetComponent<ChestInventoryHolder>();
        }

        private void Start()
        {
            _id = GetComponent<UniqueIdGenerator>().ID;
            
            SaveData(SaveGameManager.Instance.CurrentGameData);
        }

        private void OnEnable()
        {
            SaveGameManager.Instance.OnGameDataLoaded += LoadData;
        }

        private void OnDisable()
        {
            SaveGameManager.Instance.OnGameDataLoaded -= LoadData;
        }

        public override void SaveData(GameData gameData)
        {
            _inventoryData.container = _chestInventoryHolder.Container;
            gameData.chestDataDictionary.TryAdd(_id, _inventoryData);
        }

        public override void LoadData(GameData gameData)
        {
            if (gameData.chestDataDictionary.TryGetValue(_id, out var inventoryData))
            {
                _chestInventoryHolder.LoadContainer(inventoryData.container);
            }
        }
    }
}
using SaveLoadSystem;
using SaveLoadSystem.Interfaces;
using UnityEngine;

namespace ShopSystem.DataPersistence
{
    public class ShopDataHandler : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private UniqueIdGenerator uniqueIdGenerator;
        
        private string _id;
        private Shopkeeper _shopkeeper;
        private ShopData _shopData;
        
        private void Awake()
        {
            _shopkeeper = GetComponent<Shopkeeper>();
        }

        private void Start()
        {
            _id = uniqueIdGenerator.ID;
            
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
        
        public void SaveData(GameData gameData)
        {
            _shopData.shopContainer = _shopkeeper.ShopContainer;
            gameData.shopDataDictionary.TryAdd(_id, _shopData);
        }

        public void LoadData(GameData gameData)
        {
            if (gameData.shopDataDictionary.TryGetValue(_id, out var shopData))
            {
                _shopkeeper.LoadContainer(shopData.shopContainer);
            }
        }
    }
}
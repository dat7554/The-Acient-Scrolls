using Items.Components.Pickup;
using SaveLoadSystem;
using SaveLoadSystem.Interfaces;
using UnityEngine;

namespace Items.DataPersistence
{
    public class ItemPickupDataHandler : MonoBehaviour, IDataPersistence
    {
        private ItemPickup _itemPickup;
    
        private ItemPickupData _itemPickupData;
    
        private string _id;
        private bool _isEquippedOnPlayer;
    
        private void Start()
        {
            _itemPickup = GetComponent<ItemPickup>();
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

        private void OnDestroy()
        {
            SaveGameManager.Instance.CurrentGameData.activeItems.Remove(_id);
        }
        
        public void MarkAsEquipped()
        {
            _isEquippedOnPlayer = true;
        }

        public void SaveData(GameData gameData)
        {
            if (_isEquippedOnPlayer)
            {
                // Remove from activeItems if it was previously saved
                gameData.activeItems.Remove(_id);
                return;
            }
            
            _itemPickupData.itemID = _itemPickup.ItemData.ItemID;
            _itemPickupData.position = transform.position;
            _itemPickupData.rotation = transform.rotation;
            
            gameData.activeItems[_id] = _itemPickupData;
        }

        public void LoadData(GameData gameData)
        {
            if (gameData.collectedItems.Contains(_id))
            {
                Destroy(this.gameObject);
            }
        }
    }
}
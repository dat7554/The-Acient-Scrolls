using System;
using Events;
using InventorySystem.Core;
using Items.ScriptableObjects;
using UnityEngine;

namespace Characters.PlayerSystem
{
    public class PlayerInventoryHolder : InventoryHolder
    {
        [SerializeField] private int inventoryBackpackSize;
        [SerializeField] private InventoryContainer backpackContainer;
    
        private PlayerEquipment _playerEquipment;
        private PlayerStats _playerStats;
        private PlayerSoundFX _playerSoundFX;
    
        public InventoryContainer BackpackContainer => backpackContainer;

        protected override void Awake()
        {
            base.Awake();
        
            this.backpackContainer = new InventoryContainer(inventoryBackpackSize);
        }

        private void OnEnable()
        {
            GameEventManager.Instance.PlayerEventHandler.GoldGained += OnGoldReceived;
            GameEventManager.Instance.PlayerEventHandler.ItemRemoved += OnItemRemoved;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.PlayerEventHandler.GoldGained -= OnGoldReceived;
            GameEventManager.Instance.PlayerEventHandler.ItemRemoved -= OnItemRemoved;
        }
        
        public void Initialize(PlayerEquipment playerEquipment, PlayerStats playerStats, PlayerSoundFX playerSoundFX)
        {
            _playerEquipment = playerEquipment;
            _playerStats = playerStats;
            _playerSoundFX = playerSoundFX;
        }

        public bool AddItemToInventory(ItemSO itemDataToAdd, int amount)
        {
            return backpackContainer.AddItemToContainer(itemDataToAdd, amount);
        }
    
        public void LoadBackpackContainer(InventoryContainer containerToLoad)
        {
            backpackContainer = containerToLoad;
        }

        public override void UseItem(ItemSO itemDataToUse)
        {
            // TODO: consider let each SO handle logic
            if (itemDataToUse is WeaponItemSO weaponItemDataToUse)
            {
                _playerEquipment.UpdateRightEquipment(weaponItemDataToUse);
                GameEventManager.Instance.PlayerEventHandler.InvokeToggleWeaponRequested(_playerEquipment.HasDrawnWeapon);
            }
            else if (itemDataToUse is ShieldItemSO shieldItemDataToUse)
            {
                _playerEquipment.UpdateLeftEquipment(shieldItemDataToUse);
                GameEventManager.Instance.PlayerEventHandler.InvokeToggleShieldRequested(_playerEquipment.HasDrawnShield);
            }
            else if (itemDataToUse is FoodItemSO foodItemDataToUse)
            {
                _playerStats.ApplyItemEffect(itemDataToUse);
                _playerSoundFX.PlayConsumeSoundFX(foodItemDataToUse.ConsumeType);
                container.RemoveItemFromContainer(itemDataToUse, 1);
            }
        }

        private void OnGoldReceived(int quantity)
        {
            Container.ReceiveGold(quantity);
        }
        
        private void OnItemRemoved(ItemSO item, int quantity)
        {
            backpackContainer.RemoveItemFromContainer(item, quantity);
        }
    }
}

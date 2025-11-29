using System;
using System.Collections.Generic;
using System.Linq;
using Items.Core;
using Items.ScriptableObjects;
using UnityEngine;

namespace ShopSystem.Core
{
    [Serializable]
    public class ShopContainer
    {
        [SerializeField] private List<ShopSlot> shopSlots;

        [SerializeField] private int availableGold;
        [SerializeField] private float playerSellMarkUp;
        [SerializeField] private float playerBuyMarkUp;
        
        public List<ShopSlot> ShopSlots => shopSlots;
        public int NumberOfSlots => ShopSlots.Count;
        public int AvailableGold => availableGold;
        public float PlayerSellMarkUp => playerSellMarkUp;
        public float PlayerBuyMarkUp => playerBuyMarkUp;

        public ShopContainer(int size, int gold, float playerSellMarkUp, float playerBuyMarkUp)
        {
            availableGold = gold;
            this.playerSellMarkUp = playerSellMarkUp;
            this.playerBuyMarkUp = playerBuyMarkUp;
            
            SetContainerSize(size);
        }

        public bool AddItemToShopContainer(ItemSO itemDataToAdd, int quantity)
        {
            if (ContainsItem(itemDataToAdd, out var shopSlot))
            { 
                shopSlot.AddToCurrentStack(quantity);
                return true;
            }
            
            if (HasFreeSlot(out var freeSlot))
            {
                freeSlot.UpdateSlot(itemDataToAdd, quantity);
                return true;
            }
        
            return false;
        }

        public void PurchaseItem(ItemSO itemDataToSell, int quantity)
        {
            AddItemToShopContainer(itemDataToSell, quantity);
        }

        public void SellItem(ItemSO itemDataToSell, int quantity)
        {
            if (!ContainsItem(itemDataToSell, out var shopSlot)) return;
            shopSlot.RemoveFromCurrentStack(quantity);
        }

        public void ReceiveGold(int quantity)
        {
            availableGold += quantity;
        }
        
        public void SpendGold(int quantity)
        {
            availableGold -= quantity;
        }

        private void SetContainerSize(int size)
        {
            shopSlots = new List<ShopSlot>(size);
            for (int i = 0; i < size; i++)
            {
                this.shopSlots.Add(new ShopSlot());
            }
        }
        
        private bool ContainsItem(ItemSO itemDataToAdd, out ShopSlot shopSlot)
        {
            shopSlot = this.shopSlots.Find(slot => slot.ItemData == itemDataToAdd);
        
            return shopSlot is not null;
        }
        
        private bool HasFreeSlot(out ShopSlot slot)
        {
            slot = this.shopSlots.FirstOrDefault(item => item.ItemData is null);
        
            return slot != null;
        }
    }
}
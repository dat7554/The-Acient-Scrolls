using System;
using InventorySystem.ScriptableObjects;
using Items.ScriptableObjects;
using UnityEngine;

namespace Items.Core
{
    [Serializable]
    public abstract class ItemSlot : ISerializationCallbackReceiver
    {
        [NonSerialized] protected ItemSO itemData;
        
        [SerializeField] protected int itemID = -1;
        [SerializeField] protected int currentStackSize;
    
        public ItemSO ItemData => itemData;
        public int CurrentStackSize => currentStackSize;
        
        /// <summary>
        /// Updates this slot with a new item data reference and stack size,
        /// replacing any previously stored data.
        /// </summary>
        /// <param name="item">The item data to assign to the slot.</param>
        /// <param name="quantity">The number of items in the stack.</param>
        public void UpdateSlot(ItemSO item, int quantity)
        {
            // if (itemData == item)
            //     AddToCurrentStack(quantity);
            // else
            // {
            //     
            // }
            
            this.itemData = item;
            this.itemID = itemData.ItemID;
            this.currentStackSize = quantity;
        }
        
        /// <summary>
        /// Empty the slot by removing its item data reference and setting the stack size to -1.
        /// </summary>
        public void ClearSlot()
        {
            this.itemData = null;
            this.itemID = -1;
            this.currentStackSize = -1;
        }
        
        /// <summary>
        /// Assigns an item data to this slot.
        /// If this slot has the same item data type, its stack size is added.  
        /// Otherwise, overwrite this slot with the new item data and stack size.  
        /// </summary>
        /// <param name="slotToAssign">The slot whose item data and stack will be assigned.</param>
        public void AssignItem(ItemSlot slotToAssign)
        {
            if (itemData == slotToAssign.ItemData)
            {
                AddToCurrentStack(slotToAssign.CurrentStackSize);
            }
            else
            {
                this.itemData = slotToAssign.ItemData;
                this.itemID = itemData.ItemID;
                this.currentStackSize = 0;
            
                AddToCurrentStack(slotToAssign.CurrentStackSize);
            }
        }

        public void AssignItem(ItemSO itemDataToAssign, int quantity)
        {
            if (itemData != itemDataToAssign)
            {
                this.itemData = itemDataToAssign;
                this.itemID = itemData.ItemID;
                this.currentStackSize = 0;
            }

            AddToCurrentStack(quantity);
        }
        
        public void AddToCurrentStack(int quantity)
        {
            this.currentStackSize += quantity;
        }

        public void RemoveFromCurrentStack(int quantity)
        {
            this.currentStackSize -= quantity;

            if (this.currentStackSize <= 0)
                ClearSlot();
        }
        
        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            if (itemID == -1) return;
            
            var itemDatabase = Resources.Load<ItemDatabase>("Items/ItemDatabase"); // TODO: move this to some manager

            this.itemData = itemDatabase.GetItemDataFromItemDatabase(itemID);
        }
    }
}
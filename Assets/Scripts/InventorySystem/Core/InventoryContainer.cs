using System;
using System.Collections.Generic;
using System.Linq;
using Items.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem.Core
{
    [Serializable]
    public class InventoryContainer
    {
        [SerializeField] private List<InventorySlot> inventorySlots = new List<InventorySlot>();
        [SerializeField] private int gold;
        
        // Event
        public UnityAction<InventorySlot> OnInventorySlotChanged;

        // Read-only properties
        public List<InventorySlot> InventorySlots => inventorySlots;
        public int NumberOfSlots => inventorySlots.Count;
        public int Gold => gold;
    
        /// <summary>
        /// Creates a new inventory container with the given number of empty slots.
        /// </summary>
        /// <param name="size">The number of inventory slots to create.</param>
        public InventoryContainer(int size)
        {
            gold = 0;
            AddInventorySlot(size);
        }
        
        public InventoryContainer(int size, int goldAmount)
        {
            gold = goldAmount;
            AddInventorySlot(size);
        }

        private void AddInventorySlot(int size)
        {
            for (int i = 0; i < size; i++)
            {
                this.inventorySlots.Add(new InventorySlot());
            }
        }

        /// <summary>
        /// Attempts to add an item and its quantity to the inventory.
        /// </summary>
        /// <param name="itemDataToAdd">The item data to add.</param>
        /// <param name="quantity">The number of items to add.</param>
        /// <returns>
        /// True if the item was successfully added (either stacked into existing slots 
        /// or placed into a free slot), otherwise false if no space is available.
        /// </returns>
        public bool AddItemToContainer(ItemSO itemDataToAdd, int quantity)
        {
            var remainingQuantity = quantity;
            
            // TODO: if remainingQuantity exceed space left -> can NOT add -> skips to add to free slots
            // 1. Fill existing stacks first
            if (ContainsItem(itemDataToAdd, out var slots))
            {
                foreach (var slot in slots)
                {
                    slot.HasRemainingStackSpace(remainingQuantity, out var remainingSpace);
                    
                    if (remainingSpace <= 0)
                        continue;

                    var quantityCanAdd = remainingSpace > remainingQuantity ? remainingQuantity : remainingSpace;
                    slot.AddToCurrentStack(quantityCanAdd);
                    remainingQuantity -= quantityCanAdd;
                    
                    OnInventorySlotChanged?.Invoke(slot);
                    
                    if (remainingQuantity <= 0)
                        break;
                }
            }
        
            // 2. Add to free slots
            while (remainingQuantity > 0 && HasFreeSlot(out var freeSlot))
            {
                var quantityCanAdd = itemDataToAdd.MaxStackSize > remainingQuantity 
                    ? remainingQuantity : itemDataToAdd.MaxStackSize;
                
                freeSlot.UpdateSlot(itemDataToAdd, quantityCanAdd);
                remainingQuantity -= quantityCanAdd;
                
                OnInventorySlotChanged?.Invoke(freeSlot);
            }
        
            return remainingQuantity <= 0;
        }

        public void RemoveItemFromContainer(ItemSO itemDataToRemove, int quantity)
        {
            if (!ContainsItem(itemDataToRemove, out var slots)) return;
            
            var quantityToRemove = quantity;
            foreach (var slot in slots)
            {
                if (quantityToRemove <= 0)
                    return;
                
                var stackSize = slot.CurrentStackSize;
                slot.RemoveFromCurrentStack(stackSize > quantityToRemove ? quantityToRemove : stackSize);
                quantityToRemove -= stackSize;

                OnInventorySlotChanged?.Invoke(slot);
            }
        }

        public bool CheckContainerRemaining(ItemSO itemDataToCheck, out int totalRemainingQuantity)
        {
            totalRemainingQuantity = 0;
            
            if (ContainsItem(itemDataToCheck, out List<InventorySlot> slots))
            {
                foreach (var slot in slots)
                {
                    if (slot.HasRemainingStackSpace(0, out var slotRemainingQuantity))
                    {
                        totalRemainingQuantity += slotRemainingQuantity;
                    }
                }
            }
            
            totalRemainingQuantity += CountFreeSlots() * itemDataToCheck.MaxStackSize;
            return totalRemainingQuantity > 0;
        }

        public void ReceiveGold(int quantity)
        {
            gold += quantity;
        }
        
        public void SpendGold(int quantity)
        {
            gold -= quantity;
        }

        public Dictionary<ItemSO, int> GetAllItems()
        {
            var dict = new Dictionary<ItemSO, int>();

            foreach (var slot in inventorySlots)
            {
                if (slot.ItemData is null) continue;
                
                if (!dict.ContainsKey(slot.ItemData))
                    dict.Add(slot.ItemData, slot.CurrentStackSize);
                else 
                    dict[slot.ItemData] += slot.CurrentStackSize;
            }
            
            return dict;
        }

        /// <summary>
        /// Checks if the inventory contains any slots with the specified item.
        /// </summary>
        /// <param name="itemDataToAdd">The item data to search for.</param>
        /// <param name="slots">The list of inventory slots containing the item, 
        /// or an empty list if none are found.</param>
        /// <returns>True if at least one slot contains the item, otherwise false.</returns>
        private bool ContainsItem(ItemSO itemDataToAdd, out List<InventorySlot> slots)
        {
            slots = this.inventorySlots
                .Where(slot => slot.ItemData != null && slot.ItemData.ItemID == itemDataToAdd.ItemID)
                .ToList();
        
            return slots.Count > 0;
        }

        /// <summary>
        /// Finds the first available empty slot in the inventory.
        /// </summary>
        private bool HasFreeSlot(out InventorySlot slot)
        {
            slot = this.inventorySlots.FirstOrDefault(s => s.ItemData is null);
        
            return slot != null;
        }
        
        private int CountFreeSlots()
        {
            return this.inventorySlots.Count(s => s.ItemData is null);
        }
    }
}

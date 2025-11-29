using System;
using InventorySystem.ScriptableObjects;
using Items.Core;
using Items.ScriptableObjects;
using UnityEngine;

namespace InventorySystem.Core
{
    [Serializable]
    public class InventorySlot : ItemSlot
    {
        /// <summary>
        /// Creates a new inventory slot initialized with the given item data 
        /// and stack size.
        /// </summary>
        /// <param name="item">The item data to assign to the slot.</param>
        /// <param name="quantity">The number of items in the stack.</param>
        public InventorySlot(ItemSO item, int quantity)
        {
            UpdateSlot(item, quantity);
        }

        /// <summary>
        /// Creates an empty inventory slot by clearing its contents.
        /// </summary>
        public InventorySlot()
        {
            ClearSlot();
        }

        /// <summary>
        /// Checks if the slot can accommodate the given quantity without exceeding the item's maximum stack size,
        /// and outputs how much additional space remains in the stack.
        /// </summary>
        /// <param name="quantity">The number of items to check for available stack space.</param>
        /// <param name="remainingSpace">Outputs the number of items that can still fit in the stack.</param>
        /// <returns>True if the slot has enough space for the specified quantity; otherwise, false.</returns>
        public bool HasRemainingStackSpace(int quantity, out int remainingSpace)
        {
            remainingSpace = itemData.MaxStackSize - currentStackSize;
            return HasRemainingStackSpace(quantity);
        }

        /// <summary>
        /// Checks if the slot can accommodate the given quantity without exceeding the item's maximum stack size.
        /// </summary>
        /// <param name="quantity">The number of items to check for available stack space.</param>
        /// <returns>True if the slot has enough space for the specified quantity; otherwise, false.</returns>
        public bool HasRemainingStackSpace(int quantity)
        {
            // return this.itemData is null || this.itemData is not null && this.currentStackSize + quantity <= itemData.MaxStackSize;
            return this.currentStackSize + quantity <= itemData.MaxStackSize;
        }

        /// <summary>
        /// Splits the current stack into two halves.  
        /// Removes half of the items from this slot and creates a new slot with them.  
        /// </summary>
        /// <param name="halfStackSlot">The new slot containing half of the original stack.</param>
        /// <returns>True if the stack was successfully split; false if the current stack had less than 1 item.</returns>
        public bool SplitCurrentStack(out InventorySlot halfStackSlot)
        {
            if (currentStackSize <= 1)
            {
                halfStackSlot = null;
                return false;
            }

            int halfQuantity = Mathf.RoundToInt(currentStackSize / 2);
            this.RemoveFromCurrentStack(halfQuantity);
        
            halfStackSlot = new InventorySlot(itemData, halfQuantity);
            return true;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using InventorySystem.Core;
using Items.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace InventorySystem.UI
{
    public abstract class InventoryUI : MonoBehaviour
    {
        [SerializeField] private CursorSlotUI cursorSlotUI;

        protected InventoryContainer container;
        protected Dictionary<InventorySlotUI, InventorySlot> slotDictionary;
    
        public InventoryContainer Container => container;
        public Dictionary<InventorySlotUI, InventorySlot> SlotDictionary => slotDictionary;
    
        protected abstract void AssignSlots(InventoryContainer containerToAssign);

        /// <summary>
        /// Updates the UI for the specified inventory slot by finding its corresponding UI element
        /// in the slot dictionary and refreshing its display.
        /// </summary>
        /// <param name="slotToUpdate">The inventory slot that needs its UI updated.</param>
        protected virtual void UpdateSlot(InventorySlot slotToUpdate)
        {
            foreach (var slot in 
                     slotDictionary.Where(slot => slot.Value == slotToUpdate))
            {
                slot.Key.UpdateSlotUI(slotToUpdate);
                return;
            }
        }

        public void MoveItemFromCursorToContainer()
        {
            if (cursorSlotUI.CursorInventorySlot.ItemData is null) return;
            
            container.AddItemToContainer
                (
                    cursorSlotUI.CursorInventorySlot.ItemData,
                    cursorSlotUI.CursorInventorySlot.CurrentStackSize
                );
            cursorSlotUI.ClearSlot();
        }

        /// <summary>
        /// Handles logic when an inventory slot is clicked.  
        /// Supports picking up items to the cursor, placing items from the cursor to the slot,  
        /// splitting stacks, stacking items of the same type, and swapping items if different.
        /// </summary>
        /// <param name="clickedSlotUI">The inventory slot UI element that was clicked.</param>
        public void ClickSlot(InventorySlotUI clickedSlotUI)
        {
            // If slot has item data & cursor not have item data-> pick up item to cursor.
            if (clickedSlotUI.AssignedInventorySlot.ItemData is not null && cursorSlotUI.CursorInventorySlot.ItemData is null)
            {
                // If hold shift -> split stack to half
                // TODO: move this to input
                bool isShiftPressed = Keyboard.current.shiftKey.isPressed;
                if (isShiftPressed && clickedSlotUI.AssignedInventorySlot.SplitCurrentStack(out InventorySlot halfStackSlot))
                {
                    cursorSlotUI.UpdateSlot(halfStackSlot);
                
                    clickedSlotUI.UpdateSlotUI();
                    return;
                }
                else
                {
                    cursorSlotUI.UpdateSlot(clickedSlotUI.AssignedInventorySlot);
            
                    if (clickedSlotUI is EquipmentSlotUI equipmentSlotUI)
                        equipmentSlotUI.ClearEquipmentSlot();
                    else
                        clickedSlotUI.ClearSlot();
                    
                    return;
                }
            }
        
            // If slot not have item data & cursor has item data -> place item from cursor to slot.
            if (clickedSlotUI.AssignedInventorySlot.ItemData is null && cursorSlotUI.CursorInventorySlot.ItemData is not null)
            {
                if (!clickedSlotUI.CanAccept(cursorSlotUI.CursorInventorySlot.ItemData))
                {
                    Debug.Log($"Clicked slot can't accept {cursorSlotUI.CursorInventorySlot.ItemData}");
                    return;
                }
                
                clickedSlotUI.AssignedInventorySlot.AssignItem(cursorSlotUI.CursorInventorySlot);
                clickedSlotUI.UpdateSlotUI();
            
                cursorSlotUI.ClearSlot();
                return;
            }

            // If both have item...
            if (clickedSlotUI.AssignedInventorySlot.ItemData is not null &&
                cursorSlotUI.CursorInventorySlot.ItemData is not null)
            {
                // 1. If same data of item -> 
                if (clickedSlotUI.AssignedInventorySlot.ItemData == cursorSlotUI.CursorInventorySlot.ItemData)
                {
                    // 1.1. Check if slot item quantity + cursor item quantity <= slot max size...
                    if (clickedSlotUI.AssignedInventorySlot.HasRemainingStackSpace(cursorSlotUI.CursorInventorySlot.CurrentStackSize))
                    {
                        clickedSlotUI.AssignedInventorySlot.AssignItem(cursorSlotUI.CursorInventorySlot);
                        clickedSlotUI.UpdateSlotUI();
                    
                        cursorSlotUI.ClearSlot();
                    }
                    // 1.2. Else take some from mouse if slot still has free space.
                    else if (!clickedSlotUI.AssignedInventorySlot.HasRemainingStackSpace(cursorSlotUI.CursorInventorySlot.CurrentStackSize, out var remainingAmount))
                    {
                        if (remainingAmount < 1)
                        {
                            SwapClickedSlotWithCursorSlot(clickedSlotUI);
                        }
                        else
                        {
                            int remainingCursorAmount = cursorSlotUI.CursorInventorySlot.CurrentStackSize - remainingAmount;
                    
                            clickedSlotUI.AssignedInventorySlot.AddToCurrentStack(remainingAmount);
                            clickedSlotUI.UpdateSlotUI();
                    
                            var tempSlot = new InventorySlot(cursorSlotUI.CursorInventorySlot.ItemData, remainingCursorAmount);
                            cursorSlotUI.ClearSlot();
                            cursorSlotUI.UpdateSlot(tempSlot);   
                        }
                    }
                }
                // 2. Else different items -> swap slot item with cursor item.   
                else
                {
                    if (!clickedSlotUI.CanAccept(cursorSlotUI.CursorInventorySlot.ItemData))
                    {
                        Debug.Log($"Clicked slot can't accept {cursorSlotUI.CursorInventorySlot.ItemData}");
                        return;
                    }
                    
                    SwapClickedSlotWithCursorSlot(clickedSlotUI);
                }
            }
        }

        /// <summary>
        /// Swaps the item in the clicked slot with the item held by the cursor.  
        /// Creates a temporary slot to hold the cursor item, assigns the clicked slot to the cursor,  
        /// then assigns the temporary item back to the clicked slot.
        /// </summary>
        /// <param name="clickedSlotUI">The inventory slot UI element that was clicked.</param>
        private void SwapClickedSlotWithCursorSlot(InventorySlotUI clickedSlotUI)
        {
            var tempSlot = new InventorySlot(cursorSlotUI.CursorInventorySlot.ItemData, cursorSlotUI.CursorInventorySlot.CurrentStackSize);
        
            cursorSlotUI.ClearSlot();
            cursorSlotUI.UpdateSlot(clickedSlotUI.AssignedInventorySlot);
        
            clickedSlotUI.ClearSlot();
            clickedSlotUI.AssignedInventorySlot.AssignItem(tempSlot);
            clickedSlotUI.UpdateSlotUI();
        }
    }
}

using System.Collections.Generic;
using InventorySystem.Core;
using UnityEngine;

namespace InventorySystem.UI
{
    public class StaticInventoryUI : InventoryUI
    {
        [SerializeField] protected InventoryHolder holder;
        [SerializeField] protected InventorySlotUI[] slotUIArray;
    
        protected virtual void Start()
        {
            if (holder is not null)
            {
                container = holder.Container;
                container.OnInventorySlotChanged += UpdateSlot;
            }
            else
            {
                Debug.LogWarning($"{this.gameObject} needs a container to be assigned");
            }
        
            AssignSlots(container);
        }

        private void OnDestroy()
        {
            container.OnInventorySlotChanged -= UpdateSlot;
        }
        
        /// <summary>
        /// Refreshes the UI to match the current container from the holder.
        /// Call this after the container is replaced (e.g., after loading save data).
        /// </summary>
        public void RefreshUI()
        {
            if (holder is null || holder.Container is null) return;
            
            // Unsubscribe from old container if it exists
            if (container != null)
            {
                container.OnInventorySlotChanged -= UpdateSlot;
            }
            
            // Get the current container (may have been replaced by LoadContainer)
            container = holder.Container;
            
            // Subscribe to new container's events
            container.OnInventorySlotChanged += UpdateSlot;
            
            // Re-assign slots to point to the new container
            AssignSlots(container);
            
            // Update all slots to reflect current container state
            UpdateAllSlots();
        }
        
        /// <summary>
        /// Updates all UI slots to reflect the current state of the container.
        /// </summary>
        private void UpdateAllSlots()
        {
            if (container == null || slotDictionary == null) return;
            
            foreach (var (slotUI, slot) in slotDictionary)
            {
                slotUI.UpdateSlotUI(slot);
            }
        }
    
        /// <summary>
        /// Assigns an <see cref="InventoryContainer"/> to this UI by linking each
        /// <see cref="InventorySlotUI"/> in <c>slotUIArray</c> to the corresponding 
        /// <see cref="InventorySlot"/> in the container.  
        /// Validates that the container and UI have the same number of slots before assignment.
        /// </summary>
        /// <param name="containerToAssign">The inventory container to link with the UI.</param>
        protected override void AssignSlots(InventoryContainer containerToAssign)
        {
            slotDictionary = new Dictionary<InventorySlotUI, InventorySlot>();
        
            if (containerToAssign.NumberOfSlots != slotUIArray.Length)
            {
                Debug.LogError($"AssignSlots on {this.gameObject}: containerToAssign.NumberOfSlots != slotUIArray.Length");
                return;
            }
            
            for (int i = 0; i < containerToAssign.NumberOfSlots; i++)
            {
                slotDictionary.Add(slotUIArray[i], containerToAssign.InventorySlots[i]);
                slotUIArray[i].Initialize(containerToAssign.InventorySlots[i]);
            }
        }
    }
}

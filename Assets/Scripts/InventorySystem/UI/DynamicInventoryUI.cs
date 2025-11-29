using System.Collections.Generic;
using System.Linq;
using InventorySystem.Core;
using UnityEngine;

namespace InventorySystem.UI
{
    public class DynamicInventoryUI : InventoryUI
    {
        [SerializeField] protected InventorySlotUI inventorySlotUIPrefab;

        private void OnDisable()
        {
            if (container != null) container.OnInventorySlotChanged -= UpdateSlot;
        }

        public void UpdateDynamicInventoryUI(InventoryContainer containerToDisplay)
        {
            ClearSlots();
            container = containerToDisplay;
            if (container != null) container.OnInventorySlotChanged += UpdateSlot;
            AssignSlots(container);
        }

        protected override void AssignSlots(InventoryContainer containerToAssign)
        {
            slotDictionary = new Dictionary<InventorySlotUI, InventorySlot>();
        
            if (containerToAssign is null)
            {
                Debug.LogError($"AssignSlots on {this.gameObject}: containerToAssign is null");
                return;
            }
            
            for (int i = 0; i < containerToAssign.NumberOfSlots; i++)
            {
                var inventorySlotUI = Instantiate(inventorySlotUIPrefab, transform);
                slotDictionary.Add(inventorySlotUI, containerToAssign.InventorySlots[i]);
                inventorySlotUI.Initialize(containerToAssign.InventorySlots[i]);
            }
        }

        private void ClearSlots()
        {
            foreach (var item in transform.Cast<Transform>())
            {
                Destroy(item.gameObject);
            }

            if (slotDictionary is not null)
            {
                slotDictionary.Clear();
            }
        }
    }
}

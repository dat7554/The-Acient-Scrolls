using System.Collections.Generic;
using InventorySystem.Core;
using Items.Core;
using Items.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] protected InventorySlot assignedInventorySlot;
    
        [Header("Item Types Allowed")]
        [SerializeField] protected ItemType[] allowedItemTypesArray;
        
        [Header("UI Information")]
        [SerializeField] protected Image itemIcon;
        [SerializeField] protected TextMeshProUGUI itemCount;

        private HashSet<ItemType> _allowedItemTypesSet;
        private Button _button;
    
        public InventorySlot AssignedInventorySlot => assignedInventorySlot;
        public InventoryUI ParentInventoryUI { get; private set; }
    
        private void Awake()
        {
            _allowedItemTypesSet = new HashSet<ItemType>(allowedItemTypesArray);
            
            ClearSlot();
            
            itemIcon.preserveAspect = true;
        
            _button = GetComponent<Button>();
            _button?.onClick.AddListener(OnSlotUIClick);
        
            ParentInventoryUI = GetComponentInParent<InventoryUI>();
        }

        /// <summary>
        /// Initializes this slot UI with the given <see cref="InventorySlot"/> 
        /// and updates its visual state.
        /// </summary>
        /// <param name="slot">The inventory slot to bind to this UI element.</param>
        public void Initialize(InventorySlot slot)
        {
            this.assignedInventorySlot = slot;
            UpdateSlotUI();
        }

        /// <summary>
        /// Updates the slot UI to display the given <see cref="InventorySlot"/> data, 
        /// showing its icon and stack count, or clears it if empty.
        /// </summary>
        /// <param name="slot">The inventory slot whose data will be displayed.</param>
        public void UpdateSlotUI(InventorySlot slot)
        {
            if (slot.ItemData is null)
            {
                ClearSlot();
                return;
            }

            itemIcon.sprite = slot.ItemData.ItemIcon;
            itemIcon.color = Color.white;
            if (slot.CurrentStackSize > 1)
                itemCount.text = slot.CurrentStackSize.ToString();
            else
                itemCount.text = "";
        }

        /// <summary>
        /// Updates the slot UI using the currently assigned <see cref="InventorySlot"/>.
        /// Does nothing if no slot is currently assigned.
        /// </summary>
        public void UpdateSlotUI()
        {
            if (assignedInventorySlot is null) return;

            UpdateSlotUI(assignedInventorySlot);
        }

        /// <summary>
        /// Clears the currently assigned <see cref="InventorySlot"/> 
        /// and resets the UI elements (icon and stack text).
        /// </summary>
        public void ClearSlot()
        {
            assignedInventorySlot?.ClearSlot();
        
            itemIcon.sprite = null;
            itemIcon.color = Color.black;
            itemCount.text = "";
        }
        
        public bool CanAccept(ItemSO itemData)
        {
            return itemData is not null && _allowedItemTypesSet.Contains(itemData.ItemType);
        }

        private void OnSlotUIClick()
        {
            ParentInventoryUI?.ClickSlot(this);
        }
    }
}

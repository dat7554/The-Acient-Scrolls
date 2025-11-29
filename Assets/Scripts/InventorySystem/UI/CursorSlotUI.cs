using System.Collections.Generic;
using Events;
using InventorySystem.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class CursorSlotUI : MonoBehaviour
    {
        [SerializeField] private InventorySlot cursorInventorySlot;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemCount;
        
        [Header("Drop items")]
        [SerializeField] private Transform playerCharacterTransform;
        [SerializeField] private float dropOffset; // TODO: change drop logic to drop avoid unavailable area

        public InventorySlot CursorInventorySlot => cursorInventorySlot;
    
        #region Unity Methods
        
        private void Awake()
        {
            ClearSlot();
            itemIcon.preserveAspect = true;
        }

        private void OnEnable()
        {
            GameEventManager.Instance.InputEventHandler.DropItemPerformed += OnDropItemPerformed;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.InputEventHandler.DropItemPerformed -= OnDropItemPerformed;
        }

        private void Update()
        {
            FollowCursorPosition();
        }
        
        #endregion

        #region Public Methods
        
        /// <summary>
        /// Updates the cursor slot to display the given inventory slot's item, 
        /// including icon and stack count (hidden if only one).
        /// </summary>
        /// <param name="slot">The inventory slot whose data will be assigned to the cursor.</param>
        public void UpdateSlot(InventorySlot slot)
        {
            cursorInventorySlot.AssignItem(slot);
            UpdateSlot();
        }

        /// <summary>
        /// Clears the cursor slot by removing its assigned item, 
        /// resetting the icon and hiding the stack count.
        /// </summary>
        /// <remarks>
        /// Also clears the underlying cursor slot to ensure
        /// both the UI and data state remain consistent.
        /// </remarks>
        public void ClearSlot()
        {
            cursorInventorySlot.ClearSlot();
        
            itemIcon.sprite = null;
            itemIcon.color = Color.clear;
            itemCount.text = "";
        }
        
        #endregion
        
        #region Private Methods
        
        private void UpdateSlot()
        {
            itemIcon.sprite = cursorInventorySlot.ItemData.ItemIcon;
            itemIcon.color = Color.white;
            itemCount.text = cursorInventorySlot.CurrentStackSize > 1 ? 
                cursorInventorySlot.CurrentStackSize.ToString() : "";
        }
        
        private void OnDropItemPerformed()
        {
            if (cursorInventorySlot.ItemData is null || IsPointerOverUIObject()) return;
            
            var prefab = cursorInventorySlot.ItemData.ItemPrefab;
            if (prefab is not null)
            {
                Instantiate
                (
                    cursorInventorySlot.ItemData.ItemPrefab, 
                    playerCharacterTransform.position + playerCharacterTransform.forward * dropOffset, 
                    Quaternion.identity
                );
            }
                
            if (cursorInventorySlot.CurrentStackSize > 1)
            {
                cursorInventorySlot.AddToCurrentStack(-1);
                UpdateSlot();
            }
            else
            {
                ClearSlot();
            }
        }

        /// <summary>
        /// Checks if the mouse pointer is currently over a UI element
        /// by performing a raycast against the EventSystem.
        /// </summary>
        private bool IsPointerOverUIObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };

            List<RaycastResult> results =  new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        private void FollowCursorPosition()
        {
            if (cursorInventorySlot.ItemData is not null)
            {
                transform.position = Mouse.current.position.ReadValue();
            }
        }
        
        #endregion
    }
}

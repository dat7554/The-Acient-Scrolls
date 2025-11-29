using Events;
using UnityEngine;

namespace InventorySystem.UI
{
    public class EquipmentSlotUI : InventorySlotUI
    {
        public void ClearEquipmentSlot()
        {
            ClearSlot();
            
            // Equipment slots only ever have one allowed type (MeleeWeapon or Shield)
            if (allowedItemTypesArray is not null && allowedItemTypesArray.Length == 1)
            {
                GameEventManager.Instance.UIEventHandler.InvokeEquipmentSlotUnequipped(allowedItemTypesArray[0]);
            }
            else
            {
                Debug.LogWarning($"{name} EquipmentSlotUI: allowedItemTypesArray is missing or has multiple entries.");
            }
        }
    }
}
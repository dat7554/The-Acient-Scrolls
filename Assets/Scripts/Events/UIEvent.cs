using System;
using Characters.PlayerSystem.Input;
using Items.Core;

namespace Events
{
    public class UIEvent
    {
        #region Events
        
        public event Action<ActionMap> ActionMapChangeRequested;

        public event Action<ItemType> EquipmentSlotUnequipped;

        public event Action ButtonSelected;
        
        #endregion
        
        #region Event invocation methods
        
        public void InvokeActionMapChangeRequested(ActionMap actionMap) => ActionMapChangeRequested?.Invoke(actionMap);
        
        public void InvokeEquipmentSlotUnequipped(ItemType itemType) => EquipmentSlotUnequipped?.Invoke(itemType);
        
        public void InvokeButtonSelected() => ButtonSelected?.Invoke();
        
        #endregion
    }
}
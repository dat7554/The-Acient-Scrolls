using System;
using Characters.PlayerSystem;
using Items.ScriptableObjects;
using UnityEngine;

namespace Events
{
    public class PlayerEvent
    {
        #region Events
        
        public event Action<int> GoldGained;
        public event Action<ItemSO, int> ItemRemoved; 
        
        public event Action EquippedRightItemChanged;
        public event Action EquippedLeftItemChanged;
        
        public event Action<bool> ToggleWeaponRequested;
        public event Action<bool> ToggleShieldRequested;
        
        public event Action ShieldDrawn;
        public event Action ShieldSheath;
        
        public event Action<AttackType> AttackPerformed;
        public event Action BlockStarted;
        public event Action BlockEnded;
        
        public event Action<float, float> CurrentStaminaChanged;
        public event Action<float, float> CurrentHealthChanged;
        
        #endregion
        
        #region Event invocation methods
        
        public void InvokeGoldGained(int goldAmount) => GoldGained?.Invoke(goldAmount);
        public void InvokeItemRemoved(ItemSO itemData, int quantity) => ItemRemoved?.Invoke(itemData, quantity);
        
        public void InvokeEquippedRightItemChanged() => EquippedRightItemChanged?.Invoke();
        public void InvokeEquippedLeftItemChanged() => EquippedLeftItemChanged?.Invoke();
        
        public void InvokeToggleWeaponRequested(bool hasDrawnWeapon) => ToggleWeaponRequested?.Invoke(hasDrawnWeapon);
        public void InvokeToggleShieldRequested(bool hasDrawnShield) => ToggleShieldRequested?.Invoke(hasDrawnShield);
        
        public void InvokeShieldDrawn() => ShieldDrawn?.Invoke();
        public void InvokeShieldSheath() => ShieldSheath?.Invoke();
        
        public void InvokeAttackPerformed(AttackType type) => AttackPerformed?.Invoke(type);
        public void InvokeBlockStarted() => BlockStarted?.Invoke();
        public void InvokeBlockEnded() => BlockEnded?.Invoke();
        
        public void InvokeCurrentStaminaChanged(float current, float max) => CurrentStaminaChanged?.Invoke(current, max);
        public void InvokeCurrentHealthChanged(float current, float max) => CurrentHealthChanged?.Invoke(current, max);

        #endregion
    }
}

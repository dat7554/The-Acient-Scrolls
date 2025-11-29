using System;
using Events;
using UnityEngine;

namespace Characters.PlayerSystem
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerEquipment _playerEquipment;
        private PlayerStats _playerStats;

        private bool _requestedBlock;
        
        public bool IsBlocking { get; private set; }

        #region Unity Methods
    
        private void OnEnable()
        {
            GameEventManager.Instance.InputEventHandler.ToggleWeaponPerformed += HandleToggleWeaponPerformed;
            GameEventManager.Instance.InputEventHandler.AttackPerformed += HandleAttackPerformed;
            GameEventManager.Instance.InputEventHandler.BlockStarted += HandleBlockStarted;
            GameEventManager.Instance.InputEventHandler.BlockCanceled += HandleBlockCanceled;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.InputEventHandler.ToggleWeaponPerformed -= HandleToggleWeaponPerformed;
            GameEventManager.Instance.InputEventHandler.AttackPerformed -= HandleAttackPerformed;
            GameEventManager.Instance.InputEventHandler.BlockStarted -= HandleBlockStarted;
            GameEventManager.Instance.InputEventHandler.BlockCanceled -= HandleBlockCanceled;
        }

        #endregion
        
        public void Initialize(PlayerEquipment playerEquipment, PlayerStats playerStats)
        {
            _playerEquipment = playerEquipment;
            _playerStats = playerStats;
        }
        
        public void UpdateCombat(float deltaTime)
        {
            if (!_playerEquipment.HasDrawnShield) return;
            
            if (_requestedBlock && _playerStats.HasStaminaForBlock(deltaTime))
            {
                if (IsBlocking) return;
                IsBlocking = true;
                GameEventManager.Instance.PlayerEventHandler.InvokeBlockStarted();
            }
            else if (IsBlocking)
            {
                HandleBlockCanceled();
            }
        }

        #region Private Methods
        
        private void HandleToggleWeaponPerformed()
        {
            if (!_playerEquipment.HasEquippedWeapon) return;
        
            GameEventManager.Instance.PlayerEventHandler.InvokeToggleWeaponRequested(_playerEquipment.HasDrawnWeapon);
        }

        private void HandleAttackPerformed(AttackType attackType)
        {
            if (!_playerEquipment.HasEquippedWeapon) return;
        
            if (!_playerEquipment.HasDrawnWeapon)
            {
                GameEventManager.Instance.PlayerEventHandler.InvokeToggleWeaponRequested(_playerEquipment.HasDrawnWeapon);
            }
            else
            {
                GameEventManager.Instance.PlayerEventHandler.InvokeAttackPerformed(attackType);
            }
        }

        private void HandleBlockStarted()
        {
            if (!_playerEquipment.HasDrawnShield) return;
            _requestedBlock = true;
        }

        private void HandleBlockCanceled()
        {
            _requestedBlock = false;

            if (!IsBlocking) return;
            IsBlocking = false;
            GameEventManager.Instance.PlayerEventHandler.InvokeBlockEnded();
        }
        
        #endregion
    }
}

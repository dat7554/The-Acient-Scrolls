using Characters.PlayerSystem.Input.Data;
using Events;
using UnityEngine;

namespace Characters.PlayerSystem.Animators
{
    public abstract class PlayerAnimator : MonoBehaviour
    {
        // Animator parameter hashes
        protected static readonly int Horizontal          = Animator.StringToHash("Horizontal");
        protected static readonly int Vertical            = Animator.StringToHash("Vertical");
        protected static readonly int AttackIKBlend       = Animator.StringToHash("AttackIKBlend");
        protected static readonly int IsMoving            = Animator.StringToHash("IsMoving");
        protected static readonly int IsCrouching         = Animator.StringToHash("IsCrouching");
        protected static readonly int IsSprinting         = Animator.StringToHash("IsSprinting");
        protected static readonly int IsGrounded          = Animator.StringToHash("IsGrounded");
        protected static readonly int IsJumping           = Animator.StringToHash("IsJumping");
        protected static readonly int IsBlocking          = Animator.StringToHash("IsBlocking");
        protected static readonly int TriggerDrawWeapon   = Animator.StringToHash("TriggerDrawWeapon");
        protected static readonly int TriggerSheathWeapon = Animator.StringToHash("TriggerSheathWeapon");
        protected static readonly int TriggerDrawShield   = Animator.StringToHash("TriggerDrawShield");
        protected static readonly int TriggerSheathShield = Animator.StringToHash("TriggerSheathShield");
        protected static readonly int TriggerLightAttack  = Animator.StringToHash("TriggerLightAttack");
        protected static readonly int TriggerBlockImpact  = Animator.StringToHash("TriggerBlockImpact");
        protected static readonly int TriggerIdle         = Animator.StringToHash("TriggerIdle");
        protected static readonly int TriggerCombatIdle   = Animator.StringToHash("TriggerCombatIdle");
    
        protected PlayerCharacter _playerCharacter;
        protected PlayerCombat _playerCombat;
        protected PlayerEquipment _playerEquipment;
        
        protected virtual void OnEnable()
        {
            GameEventManager.Instance.PlayerEventHandler.ToggleWeaponRequested += TriggerDrawOrSheathWeaponAnimation;
            GameEventManager.Instance.PlayerEventHandler.ToggleShieldRequested += TriggerDrawOrSheathShieldAnimation;
            GameEventManager.Instance.PlayerEventHandler.AttackPerformed += TriggerAttackAnimation;

            GameEventManager.Instance.InputEventHandler.CrouchStarted += TriggerSheathWeaponAnimation;
            GameEventManager.Instance.InputEventHandler.CrouchStarted += TriggerSheathShieldAnimation;
        }

        protected virtual void OnDisable()
        {
            GameEventManager.Instance.PlayerEventHandler.ToggleWeaponRequested -= TriggerDrawOrSheathWeaponAnimation;
            GameEventManager.Instance.PlayerEventHandler.ToggleShieldRequested -= TriggerDrawOrSheathShieldAnimation;
            GameEventManager.Instance.PlayerEventHandler.AttackPerformed -= TriggerAttackAnimation;
            
            GameEventManager.Instance.InputEventHandler.CrouchStarted -= TriggerSheathWeaponAnimation;
            GameEventManager.Instance.InputEventHandler.CrouchStarted -= TriggerSheathShieldAnimation;
        }

        protected void Initialize(PlayerCharacter playerCharacter, PlayerCombat playerCombat, PlayerEquipment playerEquipment)
        {
            _playerCharacter = playerCharacter;
            _playerCombat = playerCombat;
            _playerEquipment = playerEquipment;
        }

        public abstract void UpdateAnimation(MovementInput movementInput);

        public abstract void HandleBlockImpact();
        public abstract void HandleAttackHit();
        
        protected abstract void TriggerDrawOrSheathWeaponAnimation(bool hasDrawnWeapon);
        protected abstract void TriggerDrawOrSheathShieldAnimation(bool hasDrawnShield);
        
        protected abstract void TriggerSheathWeaponAnimation();
        protected abstract void TriggerSheathShieldAnimation();

        protected abstract void TriggerAttackAnimation(AttackType type);
        
        protected bool CanTriggerDraw(bool hasDrawn)
        {
            return hasDrawn || !_playerCharacter.IsCrouching;
        }
    }
}

using Characters.PlayerSystem.AnimatorEventReceiver;
using Characters.PlayerSystem.Input.Data;
using Events;
using PlayerSystem;
using PlayerSystem.Input.Data;
using UnityEngine;

namespace Characters.PlayerSystem.Animators
{
    public class ShadowPlayerAnimator : PlayerAnimator
    {
        private Animator _shadowAnimator;
        private ShadowAnimatorEventReceiver _shadowAnimatorEventReceiver;

        private void Awake()
        {
            _shadowAnimator = GetComponent<Animator>();
            _shadowAnimatorEventReceiver = GetComponent<ShadowAnimatorEventReceiver>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GameEventManager.Instance.PlayerEventHandler.EquippedRightItemChanged += HandleEquippedRightItemChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameEventManager.Instance.PlayerEventHandler.EquippedRightItemChanged -= HandleEquippedRightItemChanged;
        }

        public void Initialize(Player player,PlayerCharacter playerCharacter, PlayerEquipment playerEquipment, PlayerCombat playerCombat)
        {
            base.Initialize(playerCharacter, playerCombat, playerEquipment);
            _shadowAnimatorEventReceiver.Initialize(player, playerEquipment);
        }

        public override void UpdateAnimation(MovementInput input)
        {
            _shadowAnimator.SetFloat(Horizontal, input.horizontalMovement.x, 0.1f, Time.deltaTime);
            _shadowAnimator.SetFloat(Vertical, input.horizontalMovement.y, 0.1f, Time.deltaTime);
            _shadowAnimator.SetBool(IsMoving, input.horizontalMovement.magnitude > 0);
        
            _shadowAnimator.SetBool(IsSprinting, _playerCharacter.IsSprinting);
            _shadowAnimator.SetBool(IsCrouching,  _playerCharacter.IsCrouching);
            _shadowAnimator.SetBool(IsGrounded, _playerCharacter.IsGrounded);
            _shadowAnimator.SetBool(IsJumping, _playerCharacter.IsJumping);
            
            _shadowAnimator.SetBool(IsBlocking, _playerCombat.IsBlocking);
        }
        
        // TODO: consider move to animator event receiver
        public void LerpRightArmIKWeightOff() { /* no operation */ }
        public void LerpRightArmIKWeightOn() { /* no operation */ }
        
        public override void HandleBlockImpact()
        {
            _shadowAnimator.SetTrigger(TriggerBlockImpact); 
        }

        public override void HandleAttackHit()
        {
            _shadowAnimator.SetTrigger(TriggerCombatIdle);
        }

        protected override void TriggerDrawOrSheathWeaponAnimation(bool hasDrawnWeapon)
        {
            if (!CanTriggerDraw(hasDrawnWeapon)) return;
            _shadowAnimator.SetTrigger(hasDrawnWeapon ? TriggerSheathWeapon : TriggerDrawWeapon);
        }

        protected override void TriggerDrawOrSheathShieldAnimation(bool hasDrawnShield) { /* no-op */ }

        protected override void TriggerSheathWeaponAnimation()
        {
            if (_playerEquipment.HasDrawnWeapon)
            {
                _shadowAnimator.SetTrigger(TriggerSheathWeapon);
            }
        }

        protected override void TriggerSheathShieldAnimation() { /* no-op */ }

        protected override void TriggerAttackAnimation(AttackType type)
        {
            if (type == AttackType.Light)
                _shadowAnimator.SetTrigger(TriggerLightAttack);
        }

        private void HandleEquippedRightItemChanged()
        {
            _shadowAnimator.SetTrigger(TriggerIdle);
        }
    }
}
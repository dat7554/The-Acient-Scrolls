using System.Collections;
using Characters.PlayerSystem.AnimatorEventReceiver;
using Characters.PlayerSystem.Input.Data;
using Events;
using PlayerSystem.Input.Data;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Characters.PlayerSystem.Animators
{
    // TODO: refactor
    public class ArmsPlayerAnimator : PlayerAnimator
    {
        [SerializeField] private ChainIKConstraint rightShoulderIKConstraint;
        [SerializeField] private TwoBoneIKConstraint rightArmIKConstraint;
        [SerializeField] private TwoBoneIKConstraint leftArmIKConstraint;
        [Space]
        [SerializeField] private float lerpSpeed = 5f;
        [SerializeField] private Transform shieldIdleTarget;
        [SerializeField] private Transform shieldBlockTarget;
        [Space] 
        [SerializeField] private float blockImpactDistance = 0.05f;
        [SerializeField] private float blockImpactDuration = 0.2f;
        [Space]
        [SerializeField] private float swordImpactDuration = 0.1f;
        [Space]
        [SerializeField] private float attackStart_ArmWeightLerpDuration = 0.2f;
        [SerializeField] private float attackEnd_ArmWeightLerpDuration = 0.2f;
        [SerializeField] private float blockStart_ArmWeightLerpDuration = 0.2f;
        [SerializeField] private float blockEnd_ArmWeightLerpDuration = 0.2f;
        
        private Animator _armsAnimator;
        private ArmsAnimatorEventReceiver _armsAnimatorEventReceiver;
        
        private Coroutine _rightArmIKCoroutine;
        private Coroutine _leftArmIKCoroutine;
        private bool _isAttacking;
        
        private Coroutine _shieldImpactCoroutine;
        private bool _isShieldImpactActive;
        
        private Coroutine _swordImpactCoroutine;
        
        private void Awake()
        {
            _armsAnimator = GetComponent<Animator>();
            _armsAnimatorEventReceiver = GetComponent<ArmsAnimatorEventReceiver>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GameEventManager.Instance.PlayerEventHandler.BlockStarted += OnBlockStart;
            GameEventManager.Instance.PlayerEventHandler.BlockEnded += OnBlockEnd;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameEventManager.Instance.PlayerEventHandler.BlockStarted -= OnBlockStart;
            GameEventManager.Instance.PlayerEventHandler.BlockEnded -= OnBlockEnd;
        }

        private void Update()
        {
            UpdateAttackIKWeight();
            UpdateShieldIKTargetPosition();
        }

        public void Initialize(Player player, PlayerCharacter playerCharacter, PlayerEquipment playerEquipment, PlayerCombat playerCombat)
        {
            base.Initialize(playerCharacter, playerCombat, playerEquipment);
            _armsAnimatorEventReceiver.Initialize(player, _playerEquipment);
        }
        
        public override void UpdateAnimation(MovementInput input)
        {
            _armsAnimator.SetFloat(Horizontal, input.horizontalMovement.x, 0.1f, Time.deltaTime);
            _armsAnimator.SetFloat(Vertical, input.horizontalMovement.y, 0.1f, Time.deltaTime);
            _armsAnimator.SetBool(IsMoving, input.horizontalMovement.magnitude > 0);
        
            _armsAnimator.SetBool(IsSprinting, _playerCharacter.IsSprinting);
            _armsAnimator.SetBool(IsGrounded, _playerCharacter.IsGrounded);
            _armsAnimator.SetBool(IsJumping, _playerCharacter.IsJumping);
            
            _armsAnimator.SetBool(IsBlocking, _playerCombat.IsBlocking);
        }
        
        public override void HandleBlockImpact()
        {
            if (_shieldImpactCoroutine != null) StopCoroutine(_shieldImpactCoroutine);
            _shieldImpactCoroutine = StartCoroutine(ShieldImpactCoroutine());
        }
        
        public override void HandleAttackHit()
        {
            if (_swordImpactCoroutine != null) StopCoroutine(_swordImpactCoroutine);
            _swordImpactCoroutine = StartCoroutine(SwordImpactCoroutine());
        }

        // Animation event
        // TODO: consider move to animator event receiver
        public void LerpRightArmIKWeightOff()
        {
            if (_rightArmIKCoroutine != null) StopCoroutine(_rightArmIKCoroutine);
            _rightArmIKCoroutine = StartCoroutine(LerpConstraintWeight
                (
                    rightArmIKConstraint, 
                    1, 
                    0, 
                    attackStart_ArmWeightLerpDuration
                ));
        }

        // Animation event
        public void LerpRightArmIKWeightOn()
        {
            if (_rightArmIKCoroutine != null) StopCoroutine(_rightArmIKCoroutine);
            _rightArmIKCoroutine = StartCoroutine(LerpConstraintWeight
                (
                    rightArmIKConstraint, 
                    0, 
                    1, 
                    attackEnd_ArmWeightLerpDuration
                ));
            
            _isAttacking = false;
        }

        protected override void TriggerDrawOrSheathWeaponAnimation(bool hasDrawnWeapon)
        {
            if (!CanTriggerDraw(hasDrawnWeapon)) return;
            
            if (hasDrawnWeapon)
            {
                TriggerSheathWeaponAnimation();
            }
            else
            {
                _armsAnimator.SetTrigger(TriggerDrawWeapon);
                
                if (_rightArmIKCoroutine != null) StopCoroutine(_rightArmIKCoroutine);
                _rightArmIKCoroutine = StartCoroutine(LerpConstraintWeight
                    (
                        rightArmIKConstraint, 
                        0, 
                        1, 
                        1
                    ));
            }
        }

        protected override void TriggerDrawOrSheathShieldAnimation(bool hasDrawnShield)
        {
            if (!CanTriggerDraw(hasDrawnShield)) return;
            
            if (hasDrawnShield)
            {
                TriggerSheathShieldAnimation();
            }
            else
            {
                GameEventManager.Instance.PlayerEventHandler.InvokeShieldDrawn();
                _armsAnimator.SetTrigger(TriggerDrawShield);
                
                if (_leftArmIKCoroutine != null) StopCoroutine(_leftArmIKCoroutine);
                _leftArmIKCoroutine = StartCoroutine(LerpConstraintWeight
                (
                    leftArmIKConstraint, 
                    0, 
                    1, 
                    0.5f
                ));
            }
        }
        
        protected override void TriggerSheathWeaponAnimation()
        {
            if (!_playerEquipment.HasDrawnWeapon) return;
            
            _armsAnimator.SetTrigger(TriggerSheathWeapon);

            if (_rightArmIKCoroutine != null) StopCoroutine(_rightArmIKCoroutine);
            _rightArmIKCoroutine = StartCoroutine(LerpConstraintWeight
            (
                rightArmIKConstraint, 
                1, 
                0, 
                1
            ));
        }

        protected override void TriggerSheathShieldAnimation()
        {
            if (!_playerEquipment.HasDrawnShield) return;
            
            GameEventManager.Instance.PlayerEventHandler.InvokeShieldSheath();
            _armsAnimator.SetTrigger(TriggerSheathShield);
            
            if (_leftArmIKCoroutine != null) StopCoroutine(_leftArmIKCoroutine);
            _leftArmIKCoroutine = StartCoroutine(LerpConstraintWeight
            (
                leftArmIKConstraint, 
                1, 
                0, 
                0.5f
            ));
        }

        protected override void TriggerAttackAnimation(AttackType type)
        {
            if (type == AttackType.Light)
            {
                _armsAnimator.SetTrigger(TriggerLightAttack); 
                _isAttacking = true;
            }
        }
        
        private void OnBlockStart()
        {
            if (_playerEquipment.HasDrawnWeapon) LerpRightArmIKWeightOff();

            _playerEquipment.CurrentLeftHandShieldComponent.StartBlock();
        }

        private void OnBlockEnd()
        {
            if (_playerEquipment.CurrentRightHandMeleeWeaponComponent is not null) LerpRightArmIKWeightOn();
            
            _playerEquipment.CurrentLeftHandShieldComponent.EndBlock();
        }

        private void UpdateAttackIKWeight()
        {
            if (!_isAttacking) return;
            float attackIKBlend = _armsAnimator.GetFloat(AttackIKBlend);
                
            switch (attackIKBlend)
            {
                case < 0.01f:
                    attackIKBlend = 0f;
                    break;
                case > 0.99f:
                    attackIKBlend = 1f;
                    break;
            }
                
            rightShoulderIKConstraint.weight = attackIKBlend;
        }

        private void UpdateShieldIKTargetPosition()
        {
            if (_isShieldImpactActive) return;
            
            var targetPosition = _playerCombat.IsBlocking ? shieldBlockTarget.localPosition : shieldIdleTarget.localPosition;
            leftArmIKConstraint.data.target.localPosition = Vector3.Lerp
            (
                leftArmIKConstraint.data.target.localPosition, 
                targetPosition, 
                lerpSpeed * Time.deltaTime
            );
        }
        
        private IEnumerator LerpConstraintWeight(IRigConstraint constraint, int from, int to, float duration)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                constraint.weight = Mathf.Lerp(from, to, t);
                yield return null;
            }
            
            constraint.weight = to;
        }
        
        private IEnumerator ShieldImpactCoroutine()
        {
            _isShieldImpactActive = true;

            var target = leftArmIKConstraint.data.target;
            if (target is null) yield break;
            
            Vector3 startPosition = shieldBlockTarget.localPosition;
            
            // compute an impact position
            Vector3 impactDirection = -transform.forward;
            Vector3 lateral = transform.right * 0.02f;
            Vector3 impactPosition = shieldBlockTarget.localPosition + impactDirection * blockImpactDistance + lateral;
            
            float hitTime = blockImpactDuration * 0.35f;
            float recoverTime = Mathf.Max(blockImpactDuration - hitTime, 0.01f);
            
            // hit: start -> impact
            float t = 0f;
            while (t < hitTime)
            {
                t += Time.deltaTime;
                float p = Mathf.SmoothStep(0f, 1f, t / hitTime);
                target.localPosition = Vector3.Lerp(startPosition, impactPosition, p);
                yield return null;
            }

            // recover: impact -> start
            t = 0f;
            while (t < recoverTime)
            {
                t += Time.deltaTime;
                float p = Mathf.SmoothStep(0f, 1f, t / recoverTime);
                target.localPosition = Vector3.Lerp(impactPosition, startPosition, p);
                yield return null;
            }
            
            target.localPosition = shieldBlockTarget.localPosition;
            
            _isShieldImpactActive = false;
        }

        private IEnumerator SwordImpactCoroutine()
        {
            _armsAnimator.speed = 0f;
            yield return new WaitForSeconds(swordImpactDuration); // small pause for recoil impact

            _armsAnimator.speed = 1f;
            _armsAnimator.SetTrigger(TriggerCombatIdle);
            LerpRightArmIKWeightOn();
        }
    }
}
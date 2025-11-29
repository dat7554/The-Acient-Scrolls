using System.Collections;
using Characters.PlayerSystem;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Characters.NPC.Friendly
{
    public class FriendlyNPCAnimator : MonoBehaviour
    {
        private static readonly int TriggerTurnRight = Animator.StringToHash("TriggerTurnRight");
        private static readonly int TriggerTurnLeft = Animator.StringToHash("TriggerTurnLeft");
        private static readonly int TriggerIdle = Animator.StringToHash("TriggerIdle");
        
        [SerializeField] private float rotateSpeed = 3f;
        [SerializeField] private MultiAimConstraint headAimConstraint;
        [SerializeField] private Transform targetAim;
        
        private Animator _animator;
        private Vector3 _defaultAimTargetPosition;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _defaultAimTargetPosition = targetAim.localPosition;
        }
        
        public IEnumerator RotateTowardsTarget(PlayerCharacter target)
        {
            const float turnThreshold = 2f;
            Vector3 aimVerticalOffset = new Vector3(0, 0.25f, 0);
            
            bool isTurnAnimationTriggered = false;
            
            while (true)
            {
                Vector3 normalizedTargetDirection = (target.transform.position - transform.position).normalized;
                normalizedTargetDirection = new Vector3(normalizedTargetDirection.x, 0, normalizedTargetDirection.z);
                
                float angle = Vector3.SignedAngle(transform.forward, normalizedTargetDirection, Vector3.up);
                if (!isTurnAnimationTriggered)
                {
                    if (angle > turnThreshold)
                    {
                        _animator.SetTrigger(TriggerTurnRight);
                    }
                    else if (angle < -turnThreshold)
                    {
                        _animator.SetTrigger(TriggerTurnLeft);
                    }
                    
                    isTurnAnimationTriggered = true;
                }
                
                if (Mathf.Abs(angle) < turnThreshold)
                {
                    break;
                }
                
                Vector3 newDirection = Vector3.RotateTowards
                    (
                        transform.forward, 
                        normalizedTargetDirection, 
                        Time.deltaTime * rotateSpeed, 
                        0
                    );
                transform.rotation = Quaternion.LookRotation(newDirection);

                Vector2 yawLimit = headAimConstraint.data.limits;
                float clampedAngle = Mathf.Clamp(angle, yawLimit.x, yawLimit.y);
                
                Vector3 clampedDirection = Quaternion.AngleAxis(clampedAngle, Vector3.up) * transform.forward;
                var desiredAimPosition = target.CenterPosition + aimVerticalOffset + clampedDirection;
                
                targetAim.position = Vector3.Lerp(targetAim.position, desiredAimPosition, Time.deltaTime * rotateSpeed);
                
                
                yield return null;
            }
            
            _animator.SetTrigger(TriggerIdle);
            targetAim.localPosition = _defaultAimTargetPosition;
        }
    }
}

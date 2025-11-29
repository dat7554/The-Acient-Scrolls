using Characters.PlayerSystem;
using UnityEngine;

namespace Characters.NPC.Enemy
{
    public class EnemyNPCSensor : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter target;
        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private LayerMask notSeeThroughLayerMask;
        [SerializeField] private Transform sensorTransform;
    
        [Header("Aims")]
        [SerializeField] private Transform targetAim;
        
        [Header("Eye Sensor")]
        [SerializeField] private float viewRadius = 15f;
        [SerializeField, Range(0, 360)] private float viewAngle = 90f;
        [SerializeField] private int maxOverlapColliders = 10;
        [SerializeField] private Collider[] overlapColliders;
    
        [Header("Gizmos")]
        [SerializeField] private bool showGizmos = false;
    
        private Vector3 _defaultAimTargetPosition;
        
        public PlayerCharacter Target => target;
    
        /// <summary>
        /// Gets the world position of the sensor used for detecting the target.
        /// </summary>
        public Vector3 SensorPosition => sensorTransform.position;

        private void Awake()
        {
            overlapColliders = new Collider[maxOverlapColliders];

            _defaultAimTargetPosition = targetAim.localPosition;
        }

        // TODO: consider check for character and character layer mask to attack villager
        /// <summary>
        /// Detects a target player character within the defined field of view and line of sight.
        /// If a valid target is found, it is assigned to `target`,
        /// otherwise `target` is set to null.
        /// </summary>
        public void DetectTargetInSight()
        {
            int numberOfOverlapColliders = Physics.OverlapSphereNonAlloc
                (
                    sensorTransform.position, 
                    viewRadius, 
                    overlapColliders, 
                    playerLayerMask
                );

            for (int i = 0; i < numberOfOverlapColliders; i++)
            {
                if (overlapColliders[i].GetComponent<PlayerCharacter>() is PlayerCharacter playerCharacter)
                {
                    Vector3 targetPosition = playerCharacter.CenterPosition;

                    if (IsTargetInSight(targetPosition))
                    {
                        target = playerCharacter;
                        Vector3 offset = 0.2f * Vector3.up;
                        targetAim.position = playerCharacter.CenterPosition + offset;
                    }
                    else
                    {
                        target = null;
                        targetAim.localPosition = _defaultAimTargetPosition;
                    }
                }
            }
        }

        /// <summary>
        /// Detects a target player character within the defined view radius.
        /// If a valid target is found, it is assigned to `target`,
        /// otherwise `target` is set to null.
        /// </summary>
        public void DetectTargetWithinViewRadius()
        {
            target = null;
            Vector3 offset = 0.2f * Vector3.up;
            targetAim.localPosition = _defaultAimTargetPosition;
        
            int numberOfOverlapColliders = Physics.OverlapSphereNonAlloc
                (
                    sensorTransform.position, 
                    viewRadius, 
                    overlapColliders, 
                    playerLayerMask
                );

            for (int i = 0; i < numberOfOverlapColliders; i++)
            {
                if (overlapColliders[i].GetComponent<PlayerCharacter>() is PlayerCharacter playerCharacter)
                {
                    target = playerCharacter;
                    targetAim.position = playerCharacter.CenterPosition + offset;
                    return;
                }
            }
        }

        public float GetAngleOfTarget(Vector3 characterForward, Vector3 targetDirection)
        {
            targetDirection.y = 0f;
            float angle = Vector3.Angle(characterForward, targetDirection);
            Vector3 cross = Vector3.Cross(characterForward, targetDirection);
            
            if (cross.y < 0) angle = -angle;
            
            return angle;
        }
    
        /// <summary>
        /// Checks whether target is visible and within field of view
        /// </summary>
        /// <param name="targetPosition">The world position of the target to check visibility for.</param>
        /// <returns>True if the target is within the FOV and there is an unobstructed line of sight; otherwise, false.</returns>
        private bool IsTargetInSight(Vector3 targetPosition)
        {
            Vector3 directionToTarget = (targetPosition - sensorTransform.position).normalized;
            bool isTargetInFOV = Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2;
        
            if (!isTargetInFOV) return false;
            
            float distanceToTarget = Vector3.Distance(sensorTransform.position, targetPosition);
            if (Physics.Raycast(sensorTransform.position, directionToTarget, out RaycastHit hitInfo, 
                    distanceToTarget, notSeeThroughLayerMask)) 
            {
                return hitInfo.collider.CompareTag("Player");
            }
            
            return false;
        }
    
        private Vector3 DirectionFromAngle(float angle, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angle += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }
    
        private void OnDrawGizmos()
        {
            if (showGizmos)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(sensorTransform.position, viewRadius);
        
                Gizmos.color = Color.red;
                var viewAngleA = DirectionFromAngle(-viewAngle/2, false);
                var viewAngleB = DirectionFromAngle(viewAngle/2, false);
                Gizmos.DrawLine(sensorTransform.position, sensorTransform.position + viewAngleA * viewRadius);
                Gizmos.DrawLine(sensorTransform.position, sensorTransform.position + viewAngleB * viewRadius);

                if (target is not null)
                    Gizmos.DrawLine(sensorTransform.position, target.CenterPosition);   
            }
        }
    }
}

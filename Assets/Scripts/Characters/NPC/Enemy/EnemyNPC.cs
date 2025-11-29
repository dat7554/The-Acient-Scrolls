using System;
using System.Collections;
using Audio;
using Characters.FSM.EnemyFSM;
using Characters.NPC.DataPersistence;
using Characters.NPC.Enemy.Patrol;
using Characters.NPC.Enemy.Spawning;
using Interfaces;
using SaveLoadSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.NPC.Enemy
{
    [RequireComponent(typeof(UniqueIdGenerator), typeof(EnemyNPCDataHandler))]
    public abstract class EnemyNPC : Character, IDamageable
    {
        private static readonly int TriggerTakeDamage = Animator.StringToHash("TriggerTakeDamage");
        private static readonly int TriggerTakeBackDamage = Animator.StringToHash("TriggerTakeBackDamage");
        private static readonly int TriggerTakeLeftDamage = Animator.StringToHash("TriggerTakeLeftDamage");
        private static readonly int TriggerTakeRightDamage = Animator.StringToHash("TriggerTakeRightDamage");
        protected readonly int TriggerCombatIdle = Animator.StringToHash("TriggerCombatIdle");

        public NavMeshAgent agent;
        public UniqueIdGenerator uniqueIDGenerator;
        public EnemyNPCDataHandler enemyNpcDataHandler;
        
        [SerializeField] private Rigidbody mainRigidbody;
        [SerializeField] private Collider mainCollider;
         
        [Header("Ragdoll")]
        [SerializeField] private GameObject ragdoll;
        [SerializeField] private Rigidbody[] ragdollRigidBodies;
        [SerializeField] private Collider[] ragdollColliders;
        
        [Header("Ground Check Settings")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckSphereRadius = 0.25f;
        
        [Header("Idle Settings")]
        public NonCombatStateMode nonCombatMode;
        
        [Header("Patrol Settings")]
        public PatrolPath patrolPath;
        [Space]
        public bool hasFoundClosestPatrolPoint;
        public bool hasCompletedPatrol;
        public bool canRepeatPatrol;
        [Space]
        public bool hasPatrolDestination;
        public int patrolDestinationPointIndex;
        public Vector3 patrolDestinationPosition;
        public float distanceToDestination;
        [Space]
        public float timeBetweenPatrols = 15f;
        public float restTimer;
        
        [Header("Sensor Settings")]
        [SerializeField] private float forgetDelay = 5f;
    
        [Header("Combat")]
        [SerializeField] private float attackCoolDown = 1.5f;
        [SerializeField] private float attackRange = 1.8f;
        [SerializeField] private float minAttackDistance;
    
        [Header("Gizmos")]
        [SerializeField] private bool showGizmos;
        
        private EnemyNPCSensor _sensor;
        private EnemyNPCStats _stats;
        protected EnemyStateMachine _stateMachine;
        
        private float _timePassedSinceLastAttack;
        private Coroutine _hitReactionRotationCoroutine;
        
        protected EnemyNPCSoundFX _enemyNpcSoundFX;
        protected Animator _animator;
        protected EnemySpawner Spawner;
        
        public GameObject Ragdoll => ragdoll;
        public float MinAttackDistance => minAttackDistance;
        public EnemyNPCSensor Sensor => _sensor;
        public Vector3 LastKnownTargetPosition { get; private set; }
        public override FactionType Faction => FactionType.Enemy;
        public bool IsGrounded => Physics.CheckSphere(transform.position, groundCheckSphereRadius, groundLayer);
        public bool IsDead {get; private set;}
        
        // TODO: refactor below
        public float AttackRange => attackRange;
        public Animator GetAnimator => _animator;
        public bool CanAttack => _timePassedSinceLastAttack >= attackCoolDown;
    
        // TODO: redo this code to private set
        public Vector3 TargetCenterPosition { get; set; }
        // TODO: redo this code to private set
        public bool HasForgottenTarget { get; set; }
    
        public void ResetAttackCooldown() => _timePassedSinceLastAttack = 0;
        public void UpdateAttackCooldown(float deltaTime) => _timePassedSinceLastAttack += deltaTime;

        private void Awake()
        {
            _stats = GetComponent<EnemyNPCStats>();
            _sensor = GetComponent<EnemyNPCSensor>();
            _stateMachine = GetComponent<EnemyStateMachine>();
            _enemyNpcSoundFX = GetComponent<EnemyNPCSoundFX>();
            
            _animator = GetComponent<Animator>();
            
            agent.updatePosition = false;
            agent.updateRotation = true;
            
            ForceReadyToAttack();
            SetRagdollActive(false);
        }

        private void Update()
        {
            isGrounded = IsGrounded;
        }

        private void OnAnimatorMove()
        {
            Vector3 rootPosition = _animator.rootPosition;
            rootPosition.y = agent.nextPosition.y;
            
            transform.position = rootPosition;
            agent.nextPosition = transform.position;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
        }

        public void SetSpawner(EnemySpawner spawner)
        {
            Spawner = spawner;
        }
        
        public void ForceReadyToAttack()
        {
            _timePassedSinceLastAttack = attackCoolDown;
        }

        /// <summary>
        /// Applies damage to this character by reducing its health, 
        /// and triggers the damage animation.
        /// </summary>
        /// <param name="damage">The quantity of damage to apply.</param>
        /// <param name="hitAngle">The angle from where the damage applied.</param>
        public void TakeDamage(float damage, float? hitAngle = null)
        {
            _stats.SetNewHealthValue(-damage);
            PlayDirectionalBasedDamageAnimation(hitAngle);
            PlayDamageSoundFX();
            PlayDamageGruntSoundFX();
            
            // Check if we should rotate towards hit direction
            if (hitAngle.HasValue && ShouldRotateToHitAngle())
            {
                StartHitReactionRotation(hitAngle.Value);
            }
        
            if (_stats.CurrentHealth == 0f)
            {
                Die();
            }
        }

        public abstract int ChooseAttackAnimation(bool isTargetTooClose);

        public virtual void SetAgentStoppingDistance(EnemyStateMachine.EnemyStateID stateID) { /* no operation */ }
        
        public override void HandleSuccessfulBlock() { /* no operation */ }
        
        public override void HandleAttackHit() { /* no operation */ }

        /// <summary>
        /// Updates the last known position of the target to specified position.
        /// </summary>
        /// <param name="position">The specified position</param>
        public void UpdateLastKnownTargetPosition(Vector3 position)
        {
            LastKnownTargetPosition = position;
        }

        /// <summary>
        /// Starts a coroutine that makes the enemy to forget its current target after a delay.
        /// </summary>
        /// <returns></returns>
        public Coroutine StartForgetTargetCoroutine()
        {
            return StartCoroutine(ForgetTargetAfterDelay());
        }

        /// <summary>
        /// Stops the currently running coroutine of forgetting the target.
        /// </summary>
        /// <returns>The coroutine to stop.</returns>
        public void EndForgetTargetCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
        
        public void RotateTowardsTarget()
        {
            RotateTowardsPosition(Sensor.Target.transform.position);
        }

        private void SetRagdollActive(bool active)
        {
            foreach (var rb in ragdollRigidBodies)
                rb.isKinematic = !active;

            foreach (var col in ragdollColliders)
                col.enabled = active;

            mainCollider.enabled = !active;
            mainRigidbody.isKinematic = active;
        }

        private void Die()
        {
            IsDead = true;
            
            agent.enabled = false;
            _animator.enabled = false;
            _stateMachine.enabled = false;
            SetRagdollActive(true); 
            
            Spawner.HandleEnemyDeath(this);
        }

        private IEnumerator ForgetTargetAfterDelay()
        {
            float elapsedTime = 0f;
            while (elapsedTime < forgetDelay)
            {
                if (_sensor.Target is not null)
                {
                    HasForgottenTarget = false;
                    yield break;
                }
            
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            HasForgottenTarget = true;
        }

        private void PlayDirectionalBasedDamageAnimation(float? hitAngle = null)
        {
            int animationToPlay = hitAngle switch
            {
                null => TriggerTakeDamage,
                <= -145f and >= -180f => TriggerTakeBackDamage,
                >= -45f and <= 45f => TriggerTakeDamage,
                >= -144f and <= -45f => TriggerTakeRightDamage,
                >= 45f and <= 144f => TriggerTakeLeftDamage,
                _ => TriggerTakeDamage
            };

            _animator?.SetTrigger(animationToPlay);
        }

        private void RotateTowardsPosition(Vector3 targetPosition, float rotateSpeed = 5f)
        {
            Vector3 normalizedTargetDirection = (targetPosition - Sensor.SensorPosition).normalized;
            normalizedTargetDirection = new Vector3(normalizedTargetDirection.x, 0, normalizedTargetDirection.z);
            Vector3 newDirection = Vector3.RotateTowards
            (
                transform.forward, 
                normalizedTargetDirection, 
                Time.deltaTime * rotateSpeed, 
                0
            );
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        
        private void PlayDamageSoundFX()
        {
            if (IsDead) return;
            AudioClip audioClip = AudioManager.Instance.GetRandomElement(AudioManager.Instance.physicalDamageSFX);
            _enemyNpcSoundFX.PlaySoundFX(audioClip);
        }
        
        private void PlayDamageGruntSoundFX()
        {
            if (IsDead) return;
            _enemyNpcSoundFX.PlayDamageGruntSoundFX();
        }
        
        private bool ShouldRotateToHitAngle()
        {
            if (_stateMachine == null) return false;
            
            // Only rotate in Idle and Patrol states
            return _stateMachine.CurrentState.StateID == EnemyStateMachine.EnemyStateID.Idle ||
                   _stateMachine.CurrentState.StateID == EnemyStateMachine.EnemyStateID.Patrol;
        }
        
        private void StartHitReactionRotation(float hitAngle)
        {
            // Stop any existing rotation coroutine
            if (_hitReactionRotationCoroutine != null)
            {
                StopCoroutine(_hitReactionRotationCoroutine);
            }
            
            _hitReactionRotationCoroutine = StartCoroutine(HitReactionRotationCoroutine(hitAngle));
            Debug.Log("HitReactionRotationCoroutine started");
        }
        
        private IEnumerator HitReactionRotationCoroutine(float hitAngle, float rotationDuration = 1.75f)
        {
            // Disable agent rotation temporarily
            bool originalRotation = agent.updateRotation;
            agent.updateRotation = false;
            
            // Stop agent and reset its path to prevent movement conflicts
            agent.isStopped = true;
            agent.ResetPath();
            
            // Calculate target rotation
            Vector3 hitDirection = Quaternion.Euler(0, hitAngle, 0) * transform.forward;
            Vector3 targetPosition = transform.position + hitDirection;
            
            // Rotate towards the hit direction
            float elapsedTime = 0f;
            while (elapsedTime < rotationDuration)
            {
                RotateTowardsPosition(targetPosition);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Re-enable agent rotation
            agent.updateRotation = originalRotation;
            agent.isStopped = false;
            _hitReactionRotationCoroutine = null;
        }

        private void OnDrawGizmos()
        {
            if (showGizmos)
            {
                Gizmos.color = Color.red;
                if (_sensor is not null)
                    Gizmos.DrawWireSphere(_sensor.SensorPosition, attackRange);
        
                if (agent is not null)
                {
                    if (!agent.hasPath)
                        return;
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(agent.destination, 0.5f);
                }
            }
        }
    }
}

using UnityEngine;

namespace Characters.FSM.EnemyFSM.States
{
    public class AttackState : EnemyState
    {
        private float _distanceToTarget;
        private bool _isTargetTooClose;

        public AttackState(EnemyContext context,  EnemyStateMachine.EnemyStateID enemyStateID) 
            : base(context, enemyStateID)
        {
            Context = context;
        }
    
        public override void EnterState()
        {
            Context.Npc.agent.isStopped = true;
            Context.Npc.agent.velocity = Vector3.zero;
            Context.Npc.UpdateLastKnownTargetPosition(Context.Npc.Sensor.Target.CenterPosition);
            Context.Npc.ForceReadyToAttack();

            CheckTargetDistance();
            HandleAttack(_isTargetTooClose);
        }

        public override void ExecuteState()
        {
            Context.Npc.UpdateAttackCooldown(Time.deltaTime);
            Context.Npc.Sensor.DetectTargetWithinViewRadius();

            if (Context.Npc.Sensor.Target is not null)
            {
                Context.Npc.RotateTowardsTarget();
                CheckTargetDistance();
            
                if (_distanceToTarget > Context.Npc.AttackRange)
                {
                    Context.Npc.UpdateLastKnownTargetPosition(Context.Npc.Sensor.Target.CenterPosition);
                    return;
                }
            }
        
            HandleAttack(_isTargetTooClose);
            UpdateMovementAnimation();
        }

        public override void ExitState()
        {
        
        }

        public override EnemyStateMachine.EnemyStateID GetNextState()
        {
            if (Context.Npc.Sensor.Target is null)
                return EnemyStateMachine.EnemyStateID.Idle;
            
            if (_distanceToTarget > Context.Npc.AttackRange)
                return EnemyStateMachine.EnemyStateID.Chase;

            return StateID;
        }

        private void CheckTargetDistance()
        {
            _distanceToTarget = Vector3.Distance(Context.Npc.Sensor.SensorPosition, Context.Npc.Sensor.Target.CenterPosition);
            _isTargetTooClose = _distanceToTarget < Context.Npc.MinAttackDistance;
        }
    
        private void HandleAttack(bool isTargetTooClose)
        {
            if (!Context.Npc.CanAttack) return;
            
            Context.Npc.ResetAttackCooldown();
            Context.Npc.GetAnimator.SetTrigger(Context.Npc.ChooseAttackAnimation(isTargetTooClose));
        }
    }
}
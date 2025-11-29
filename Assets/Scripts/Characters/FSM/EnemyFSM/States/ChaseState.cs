using UnityEngine;

namespace Characters.FSM.EnemyFSM.States
{
    public class ChaseState : EnemyState
    {
        private Coroutine _forgetTargetCoroutine;
        
        public ChaseState(EnemyContext context, EnemyStateMachine.EnemyStateID enemyStateID) 
            : base(context, enemyStateID)
        {
            Context = context;
        }
    
        public override void EnterState()
        {
            Context.Npc.UpdateLastKnownTargetPosition(Context.Npc.Sensor.Target.CenterPosition);
            Context.Npc.RotateTowardsTarget();
            Context.Npc.agent.isStopped = false;
            Context.Npc.SetAgentStoppingDistance(StateID);
            Context.Npc.agent.SetDestination(Context.Npc.LastKnownTargetPosition);
        }
    
        public override void ExecuteState()
        {
            Context.Npc.Sensor.DetectTargetWithinViewRadius();
        
            // If no target ...
            if (Context.Npc.Sensor.Target is null)
            {
                // Start coroutine if null
                if (_forgetTargetCoroutine is null)
                {
                    _forgetTargetCoroutine = Context.Npc.StartForgetTargetCoroutine();
                }

                if (Context.Npc.HasForgottenTarget || HasAgentArrived())
                {
                    Context.Npc.agent.ResetPath();
                }
            
                if (!Context.Npc.HasForgottenTarget && !HasAgentArrived())
                {
                    Context.Npc.agent.SetDestination(Context.Npc.LastKnownTargetPosition);
                }
            }
            // else has target.
            else
            {
                if (_forgetTargetCoroutine != null)
                {
                    Context.Npc.EndForgetTargetCoroutine(_forgetTargetCoroutine);
                    _forgetTargetCoroutine = null;
                    Context.Npc.HasForgottenTarget = false;
                }
            
                // Remember last target position
                Context.Npc.UpdateLastKnownTargetPosition(Context.Npc.Sensor.Target.CenterPosition);
        
                // Move to actual target
                Context.Npc.agent.SetDestination(Context.Npc.Sensor.Target.CenterPosition);
            }
        
            UpdateMovementAnimation();
        }

        public override void ExitState()
        {
            Context.Npc.agent.ResetPath();
        
            if (_forgetTargetCoroutine != null)
            {
                Context.Npc.EndForgetTargetCoroutine(_forgetTargetCoroutine);
                _forgetTargetCoroutine = null;
                Context.Npc.HasForgottenTarget = false;
            }
        }
        
        public override EnemyStateMachine.EnemyStateID GetNextState()
        {
            bool hasTarget = Context.Npc.Sensor.Target is not null;
            bool hasArrived = HasAgentArrived();
            
            if (!hasTarget)
                return hasArrived ? EnemyStateMachine.EnemyStateID.Idle : StateID;
            
            bool forgotTarget = Context.Npc.HasForgottenTarget;
            if (forgotTarget)
                return EnemyStateMachine.EnemyStateID.Idle;
            
            bool isInAttackRange = DistanceToTarget() <= Context.Npc.AttackRange;
            if (isInAttackRange && hasArrived)
                return EnemyStateMachine.EnemyStateID.Attack;
            
            return StateID;
        }

        private bool HasAgentArrived()
        {
            return !Context.Npc.agent.pathPending &&
                   Context.Npc.agent.remainingDistance <= Context.Npc.agent.stoppingDistance &&
                   Context.Npc.agent.velocity.sqrMagnitude < 0.01f;
        }

        private float DistanceToTarget()
        {
            return Vector3.Distance(Context.Npc.Sensor.SensorPosition, Context.Npc.Sensor.Target.CenterPosition);
        }
    }
}

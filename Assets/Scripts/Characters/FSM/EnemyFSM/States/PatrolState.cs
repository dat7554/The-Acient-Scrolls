using UnityEngine;

namespace Characters.FSM.EnemyFSM.States
{
    public class PatrolState : EnemyState
    {
        public PatrolState(EnemyContext context, EnemyStateMachine.EnemyStateID enemyStateID) 
            : base(context, enemyStateID)
        {
            Context = context;
        }

        public override void EnterState()
        {
            Context.Npc.SetAgentStoppingDistance(StateID);
        }

        public override void ExecuteState()
        {
            Context.Npc.Sensor.DetectTargetInSight();
            Patrol();
        }

        public override void ExitState()
        {
            Context.Npc.hasFoundClosestPatrolPoint = false;
            Context.Npc.hasCompletedPatrol = false;
            Context.Npc.hasPatrolDestination = false;
            Context.Npc.patrolDestinationPointIndex = -1;
            Context.Npc.patrolDestinationPosition = Context.Npc.transform.position;
                    
            Context.Npc.restTimer = 0f;
        }

        public override EnemyStateMachine.EnemyStateID GetNextState()
        {
            if (Context.Npc.Sensor.Target is not null)
            {
                return EnemyStateMachine.EnemyStateID.Chase;
            }

            if ((Context.Npc.hasCompletedPatrol && !Context.Npc.canRepeatPatrol) || !Context.Npc.patrolPath)
            {
                return EnemyStateMachine.EnemyStateID.Idle;
            }
            
            return StateID;
        }
        
        private void Patrol()
        {
            if (!Context.Npc.IsGrounded) return;

            RestAfterCompletePatrol();
            HandlePatrolDestination();
            UpdatePatrolDestination();
            UpdateMovementAnimation();
        }

        private void RestAfterCompletePatrol()
        {
            if (!Context.Npc.hasCompletedPatrol || !Context.Npc.canRepeatPatrol) return;
            
            if (Context.Npc.timeBetweenPatrols > Context.Npc.restTimer)
            {
                Context.Npc.restTimer += Time.deltaTime;
            }
            else
            {
                Context.Npc.hasCompletedPatrol = false;
                Context.Npc.hasPatrolDestination = false;
                Context.Npc.patrolDestinationPointIndex = -1;
                Context.Npc.patrolDestinationPosition = Context.Npc.transform.position;
                    
                Context.Npc.restTimer = 0f;
            }
        }

        private void HandlePatrolDestination()
        {
            // If currently has a patrol destination...
            if (Context.Npc.hasPatrolDestination)
            {
                Context.Npc.distanceToDestination = Vector3.Distance
                (
                    Context.Npc.transform.position,
                    Context.Npc.patrolDestinationPosition
                );

                // If arrive at a patrol point
                if (Context.Npc.distanceToDestination <= Context.Npc.agent.stoppingDistance)
                {
                    Context.Npc.hasPatrolDestination = false;
                }
            }
            // Else not have a patrol destination.
            else
            {
                if (!Context.Npc.hasCompletedPatrol)
                    Context.Npc.patrolDestinationPointIndex++;
                
                if (Context.Npc.patrolDestinationPointIndex > Context.Npc.patrolPath.PatrolPoints.Count - 1)
                {
                    Context.Npc.hasCompletedPatrol = true;
                    return;
                }

                if (!Context.Npc.hasFoundClosestPatrolPoint)
                {
                    Context.Npc.hasFoundClosestPatrolPoint = true;
                    
                    int closestIndex = -1;
                    float closestDistance = Mathf.Infinity;

                    for (int i = 0; i < Context.Npc.patrolPath.PatrolPoints.Count; i++)
                    {
                        float distanceFromPatrolPoint = Vector3.Distance
                            (
                                Context.Npc.transform.position, 
                                Context.Npc.patrolPath.PatrolPoints[i]
                            );

                        if (!(distanceFromPatrolPoint < closestDistance)) continue;
                        closestIndex = i;
                        closestDistance = distanceFromPatrolPoint;
                    }
                    
                    Context.Npc.patrolDestinationPointIndex = closestIndex;
                    Context.Npc.patrolDestinationPosition = Context.Npc.patrolPath.PatrolPoints[closestIndex];
                }
                else
                {
                    Context.Npc.patrolDestinationPosition = Context.Npc.patrolPath.PatrolPoints[Context.Npc.patrolDestinationPointIndex];
                }
                
                Context.Npc.hasPatrolDestination = true;
            }
        }

        // TODO: optimize - not necessarily to run every frame
        private void UpdatePatrolDestination()
        {
            if (!Context.Npc.hasPatrolDestination) return;
         
            // NavMeshPath path = new NavMeshPath();
            // Context.Npc.agent.CalculatePath(Context.Npc.patrolDestinationPosition, path);
            // Context.Npc.agent.SetPath(path);
            Context.Npc.agent.SetDestination(Context.Npc.patrolDestinationPosition);
        }
    }
}

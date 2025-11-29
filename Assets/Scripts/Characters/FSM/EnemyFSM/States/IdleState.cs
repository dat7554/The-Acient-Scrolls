namespace Characters.FSM.EnemyFSM.States
{
    public class IdleState : EnemyState
    {
        public IdleState(EnemyContext context,  EnemyStateMachine.EnemyStateID enemyStateID) 
            : base(context, enemyStateID)
        {
            Context = context;
        }
    
        public override void EnterState()
        {
        
        }

        public override void ExecuteState()
        {
            Context.Npc.Sensor.DetectTargetInSight();
        }

        public override void ExitState()
        {
            
        }

        public override EnemyStateMachine.EnemyStateID GetNextState()
        {
            if (Context.Npc.Sensor.Target is not null)
            {
                return EnemyStateMachine.EnemyStateID.Chase;
            }
            
            if (Context.Npc.nonCombatMode == NonCombatStateMode.Patrol)
            {
                return EnemyStateMachine.EnemyStateID.Patrol;
            }
            
            return StateID;
        }
    }
}

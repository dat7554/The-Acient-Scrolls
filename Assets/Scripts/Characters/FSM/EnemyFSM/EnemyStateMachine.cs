using Characters.FSM.Core;
using Characters.FSM.EnemyFSM.States;
using Characters.NPC.Enemy;

namespace Characters.FSM.EnemyFSM
{
    public class EnemyStateMachine : StateMachine<EnemyStateMachine.EnemyStateID>
    {
        public enum EnemyStateID
        {
            Idle,
            Patrol,
            Chase,
            Attack
        }

        private EnemyNPC _enemyNpc;
        private EnemyContext _context;

        private void Awake()
        {
            _enemyNpc = GetComponent<EnemyNPC>();
            
            _context = new EnemyContext(_enemyNpc);
            InitializeStates();
        }
        
        private void InitializeStates()
        {
            States.Add(EnemyStateID.Idle, new IdleState(_context, EnemyStateID.Idle));
            States.Add(EnemyStateID.Patrol, new PatrolState(_context, EnemyStateID.Patrol));
            States.Add(EnemyStateID.Chase, new ChaseState(_context, EnemyStateID.Chase));
            States.Add(EnemyStateID.Attack, new AttackState(_context, EnemyStateID.Attack));
            
            CurrentState = States[EnemyStateID.Idle];
        }
    }
}

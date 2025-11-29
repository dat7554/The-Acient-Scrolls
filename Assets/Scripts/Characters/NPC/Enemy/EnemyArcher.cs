using Characters.FSM.EnemyFSM;
using Items.Components.Weapons;
using UnityEngine;

namespace Characters.NPC.Enemy
{
    public class EnemyArcher : EnemyNPC
    {
        private static readonly int TriggerLoad = Animator.StringToHash("TriggerLoad");
        private static readonly int TriggerRelease = Animator.StringToHash("TriggerRelease");
        
        [Header("Equipment")]
        [SerializeField] private RangeWeapon leftRangeWeapon;
        [SerializeField] private Arrow rightArrow;

        public RangeWeapon LeftRangeWeapon => leftRangeWeapon;
        
        private void Start()
        {
            leftRangeWeapon?.OnEquip();
            rightArrow?.OnEquip();
        }

        public override int ChooseAttackAnimation(bool isTargetTooClose)
        {
            return TriggerLoad;
        }

        public override void SetAgentStoppingDistance(EnemyStateMachine.EnemyStateID stateID)
        {
            switch (stateID)
            {
                case EnemyStateMachine.EnemyStateID.Patrol:
                    agent.stoppingDistance = 1.85f;
                    break;
                case EnemyStateMachine.EnemyStateID.Chase:
                    agent.stoppingDistance = 7f;
                    break;
                default:
                    agent.stoppingDistance = 1.85f;
                    break;
            }
        }
    }
}

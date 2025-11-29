using Characters.FSM.Core;
using UnityEngine;

namespace Characters.FSM.EnemyFSM
{
    public abstract class EnemyState : BaseState<EnemyStateMachine.EnemyStateID>
    {
        protected EnemyContext Context;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int ShouldMove = Animator.StringToHash("ShouldMove");

        private Vector2 _smoothDeltaPosition;
        private Vector2 _velocity;
        
        protected EnemyState(EnemyContext context, EnemyStateMachine.EnemyStateID enemyStateID) : base(enemyStateID)
        {
            Context = context;
        }
        
        protected void UpdateMovementAnimation()
        {
            // Compute world-space delta between NavMeshAgent and NPC transform
            Vector3 worldDeltaPosition = Context.Npc.agent.nextPosition - Context.Npc.transform.position;
            worldDeltaPosition.y = 0f;

            // Convert world delta to local-space delta
            float dotX = Vector3.Dot(Context.Npc.transform.right, worldDeltaPosition);
            float dotY = Vector3.Dot(Context.Npc.transform.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2(dotX, dotY);

            // Smooth the delta
            float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
            _smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);
            
            // Compute local velocity & slow down when close to destination
            _velocity = _smoothDeltaPosition / Time.deltaTime;
            if (Context.Npc.agent.remainingDistance <= Context.Npc.agent.stoppingDistance)
            {
                _velocity = Vector2.Lerp
                    (
                        Vector2.zero, 
                        _velocity, 
                        Context.Npc.agent.remainingDistance/Context.Npc.agent.stoppingDistance
                    );
            }

            // Determine if NPC should move
            bool shouldMove = _velocity.magnitude > 0.5f
                && Context.Npc.agent.remainingDistance > Context.Npc.agent.stoppingDistance;
            
            // Update animator parameters
            Context.Npc.GetAnimator.SetBool(ShouldMove, shouldMove);
            Context.Npc.GetAnimator.SetFloat(Speed, _velocity.magnitude);
            
            // Adjust transform position smoothly to keeps visual (animated) and logical (navigation) positions aligned
            float deltaMagnitude = worldDeltaPosition.magnitude;
            if (deltaMagnitude > Context.Npc.agent.radius / 2f)
            {
                Context.Npc.transform.position = Vector3.Lerp
                    (
                        Context.Npc.GetAnimator.rootPosition,
                        Context.Npc.agent.nextPosition,
                        smooth
                    );
            }
        }
    }
}
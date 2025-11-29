using Characters.NPC.Enemy;

namespace Characters.FSM.EnemyFSM
{
    public class EnemyContext
    {
        public EnemyNPC Npc { get; private set; }

        public EnemyContext(EnemyNPC npc)
        {
            Npc = npc;
        }
    }
}
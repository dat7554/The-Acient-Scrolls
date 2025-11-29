using UnityEngine;

namespace Characters.NPC.Enemy.Spawning
{
    public class DetectCollider : MonoBehaviour
    {
        [SerializeField] private Collider detectCollider;
        [SerializeField] private EnemySpawner spawner;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            spawner.SpawnEnemies();
        }
    }
}

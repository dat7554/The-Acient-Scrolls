using System.Collections.Generic;
using Characters.NPC.DataPersistence;
using Characters.NPC.Enemy.Patrol;
using SaveLoadSystem;
using UnityEngine;

namespace Characters.NPC.Enemy.Spawning
{
    [RequireComponent(typeof(UniqueIdGenerator), typeof(EnemySpawnerDataHandler))]
    public class EnemySpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyNPC enemyToSpawn;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private GameObject[] spawnPoints;
        
        [Header("Configs")]
        [SerializeField] private float delayRagdollDestroyInSeconds = 180f;

        private string _spawnerID;
        private bool _hasSpawned;
        private Dictionary<string, EnemyNPC> _spawnedEnemies = new ();
        
        public string SpawnerID => _spawnerID;
        public bool HasSpawned => _hasSpawned;
        
        private void Awake()
        {
            _spawnerID = GetComponent<UniqueIdGenerator>().ID;
        }

        public void SpawnEnemies()
        {
            if (_hasSpawned) return;
            
            if (spawnPoints is null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("Spawn positions array is null or empty");
                return;
            }
            
            _spawnedEnemies.Clear();

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                GameObject spawnPoint = spawnPoints[i];
                EnemyNPC enemyNpc = Instantiate(enemyToSpawn, spawnPoint.transform.position, spawnPoint.transform.rotation);
                enemyNpc.SetSpawner(this);
                enemyNpc.patrolPath = patrolPath;
                
                string enemyID = $"{_spawnerID}_spawn_{i}";
                enemyNpc.uniqueIDGenerator.ForceSetID(enemyID);
                _spawnedEnemies.Add(enemyID, enemyNpc);
            }
            
            _hasSpawned = true;
        }

        public void SpawnEnemiesWithIDs(List<string> savedEnemyIDs)
        {
            if (_hasSpawned) return;
            
            if (spawnPoints is null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("Spawn positions array is null or empty");
                return;
            }
            
            if (savedEnemyIDs is null || savedEnemyIDs.Count == 0)
            {
                Debug.LogWarning("No saved enemy IDs provided");
                return;
            }
            
            _spawnedEnemies.Clear();

            for (int i = 0; i < savedEnemyIDs.Count && i < spawnPoints.Length; i++)
            {
                GameObject spawnPoint = spawnPoints[i];
                string enemyID = savedEnemyIDs[i];
                
                EnemyNPC enemyNpc = Instantiate(enemyToSpawn, spawnPoint.transform.position, spawnPoint.transform.rotation);
                enemyNpc.SetSpawner(this);
                enemyNpc.patrolPath = patrolPath;
                
                enemyNpc.uniqueIDGenerator.ForceSetID(enemyID);
                _spawnedEnemies.Add(enemyID, enemyNpc);
                
                enemyNpc.enemyNpcDataHandler.LoadData(SaveGameManager.Instance.CurrentGameData);
            }
            
            _hasSpawned = true;
        }

        public void DespawnEnemies()
        {
            foreach (var enemyNpc in _spawnedEnemies.Values)
            {
                Destroy(enemyNpc.gameObject);
            }
            
            _spawnedEnemies.Clear();
            _hasSpawned = false;
        }

        public List<string> GetSpawnedEnemiesIDs()
        {
            return new List<string>(_spawnedEnemies.Keys);
        }

        public void HandleEnemyDeath(EnemyNPC enemyNpc)
        {
            string enemyID = enemyNpc.uniqueIDGenerator.ID;
            _spawnedEnemies.Remove(enemyID);
            
            Destroy(enemyNpc.gameObject, delayRagdollDestroyInSeconds);
            
            // Destroy(enemyNpc.gameObject);
            //
            // var instantiatedRagdoll = Instantiate
            // (
            //     enemyNpc.Ragdoll, 
            //     enemyNpc.transform.position, 
            //     enemyNpc.transform.rotation
            // );
            // Destroy(instantiatedRagdoll, delayRagdollDestroyInSeconds);
        }
    }
}

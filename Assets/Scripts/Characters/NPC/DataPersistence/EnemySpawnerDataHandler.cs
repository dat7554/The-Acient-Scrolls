using System.Collections.Generic;
using Characters.NPC.Enemy.Spawning;
using SaveLoadSystem;
using SaveLoadSystem.Interfaces;
using UnityEngine;

namespace Characters.NPC.DataPersistence
{
    public class EnemySpawnerDataHandler : MonoBehaviour, IDataPersistence
    {
        private EnemySpawner _enemySpawner;
        private EnemySpawnerData _enemySpawnerData;

        private void Awake()
        {
            _enemySpawner = GetComponent<EnemySpawner>();
        }

        private void OnEnable()
        {
            SaveGameManager.Instance.OnGameDataLoaded += LoadData;
        }

        private void OnDisable()
        {
            SaveGameManager.Instance.OnGameDataLoaded -= LoadData;
        }
        
        public void SaveData(GameData gameData)
        {
            List<string> spawnedEnemyIDs = _enemySpawner.GetSpawnedEnemiesIDs();
            _enemySpawnerData = new EnemySpawnerData(_enemySpawner.HasSpawned, spawnedEnemyIDs);
            
            gameData.enemySpawnerDataDictionary[_enemySpawner.SpawnerID] = _enemySpawnerData;
        }

        public void LoadData(GameData gameData)
        {
            if (gameData.enemySpawnerDataDictionary.TryGetValue(_enemySpawner.SpawnerID, out var enemySpawnerData))
            {
                // No need to despawn with current way of loads game world in game manager
                // _enemySpawner.DespawnEnemies();
                
                if (enemySpawnerData.hasSpawned &&
                    enemySpawnerData.spawnedEnemyIDs != null && enemySpawnerData.spawnedEnemyIDs.Count > 0)
                {
                    _enemySpawner.SpawnEnemiesWithIDs(enemySpawnerData.spawnedEnemyIDs);
                }
            }
        }
    }
}

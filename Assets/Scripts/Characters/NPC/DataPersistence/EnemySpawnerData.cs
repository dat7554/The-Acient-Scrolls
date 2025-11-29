using System;
using System.Collections.Generic;

namespace Characters.NPC.DataPersistence
{
    [Serializable]
    public class EnemySpawnerData
    {
        public bool hasSpawned;
        public List<string> spawnedEnemyIDs;

        public EnemySpawnerData(bool hasSpawned, List<string> spawnedEnemyIDs)
        {
            this.hasSpawned = hasSpawned;
            this.spawnedEnemyIDs = spawnedEnemyIDs ?? new List<string>();
        }
    }
}

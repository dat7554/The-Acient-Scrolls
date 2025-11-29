using System;
using UnityEngine;

namespace Characters.NPC.DataPersistence
{
    [Serializable]
    public struct EnemyNPCData
    {
        // Position & Rotation
        public Vector3 position;
        
        // Stats
        public float currentHealth; 
        public float maxHealth;

        public EnemyNPCData(Vector3 position, float currentHealth, float maxHealth)
        {
            this.position = position;
            this.currentHealth = currentHealth;
            this.maxHealth = maxHealth;
        }
    }
}

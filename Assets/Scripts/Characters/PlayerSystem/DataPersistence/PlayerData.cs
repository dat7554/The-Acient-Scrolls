using System;
using InventorySystem.DataPersistence;
using UnityEngine;

namespace Characters.PlayerSystem.DataPersistence
{
    [Serializable]
    public struct PlayerData
    {
        // Position & Rotation
        public Vector3 cameraRotation;
        public Vector3 position;
        public Quaternion characterRotation;
    
        // Stats
        public float currentHealth; 
        public float maxHealth;
    
        public float currentStamina;
        public float maxStamina;
    
        // Inventory
        public InventoryData hotbarInventoryData;
        public InventoryData backpackInventoryData;
    }
}
using System;
using Items.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem.Core
{
    [Serializable]
    public abstract class InventoryHolder : MonoBehaviour
    {
        [SerializeField] protected int gold;
        
        [Header("Containers")]
        [SerializeField] protected int inventorySize;
        [SerializeField] protected InventoryContainer container;
    
        // Read-only properties
        public InventoryContainer Container => container;
    
        protected virtual void Awake()
        {
            container = new InventoryContainer(inventorySize, gold);
        }
        
        public void LoadContainer(InventoryContainer containerToLoad)
        {
            container = containerToLoad;
        }

        public virtual void UseItem(ItemSO item)
        {
            
        }
    }
}
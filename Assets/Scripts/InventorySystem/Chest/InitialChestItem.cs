using System;
using Items.ScriptableObjects;

namespace InventorySystem.Chest
{
    [Serializable]
    public struct InitialChestItem
    {
        public ItemSO itemData;
        public int quantity;
    }
}

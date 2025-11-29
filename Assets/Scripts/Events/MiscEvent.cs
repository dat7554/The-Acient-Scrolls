using System;
using Items.ScriptableObjects;

namespace Events
{
    public class MiscEvent
    {
        public event Action<ItemSO> ItemCollected;
        
        public void InvokeItemCollected(ItemSO itemData) => ItemCollected?.Invoke(itemData);
    }
}
using Items.Core;
using UnityEngine;

namespace Items.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Quest Item", menuName = "InventorySystem/Items/Quest Item")]
    public class QuestItemSO : ItemSO
    {
        private void Awake()
        {
            itemType = ItemType.Quest;
            maxStackSize = 10;
        }
        
        public override void UseItem()
        {
            
        }
    }
}

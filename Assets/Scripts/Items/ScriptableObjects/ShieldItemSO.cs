using Items.Core;
using UnityEngine;

namespace Items.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Shield Item", menuName = "InventorySystem/Items/Shield Item")]
    public class ShieldItemSO : ItemSO
    {
        private void Awake()
        {
            itemType = ItemType.Shield;
            maxStackSize = 1;
        }
        
        public override void UseItem()
        {
            Debug.Log("Use Shield Item...");
        }
    }
}
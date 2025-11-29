using Items.Core;
using UnityEngine;

namespace Items.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Food Item", menuName = "InventorySystem/Items/Food Item")]
    public class FoodItemSO : ItemSO
    {
        [SerializeField] private ConsumeType consumeType;
        [SerializeField] private float healthRestoreAmount;
        
        public ConsumeType ConsumeType => consumeType;
        public float HealthRestoreAmount => healthRestoreAmount;

        private void Awake()
        {
            itemType = ItemType.Food;
            maxStackSize = 10;
        }
        
        public override void UseItem()
        {
            Debug.Log("Use Food Item...");
        }
    }
}

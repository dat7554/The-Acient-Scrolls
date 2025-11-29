using Items.Core;
using UnityEngine;

namespace Items.ScriptableObjects
{
    [CreateAssetMenu(fileName = "MeleeWeapon Item", menuName = "InventorySystem/Items/MeleeWeapon Item")]
    public class WeaponItemSO : ItemSO
    {
        [SerializeField] private float damage;
    
        public float Damage => damage;

        private void Awake()
        {
            itemType = ItemType.Weapon;
            maxStackSize = 1;
        }

        public override void UseItem()
        {
            Debug.Log("Use MeleeWeapon Item...");
        }
    }
}

using Items.Core;
using UnityEngine;

namespace Items.ScriptableObjects
{
    /// <summary>
    /// This is a base Scriptable Object that defines what an item is.
    /// It can be inherited for all other type of items (e.g. food, weapon,...).
    /// </summary>
    public abstract class ItemSO : ScriptableObject
    {
        [SerializeField] protected int itemID = -1;
        [SerializeField] protected string itemName;
        [SerializeField] protected ItemType itemType;
        [SerializeField] protected GameObject itemPrefab;
        [SerializeField] protected Sprite itemIcon;
        [SerializeField] protected int maxStackSize;
        [SerializeField] protected int goldValue;
        [SerializeField, TextArea(5,10)] protected string itemDescription;
    
        public int ItemID => itemID;
        public string ItemName => itemName;
        public ItemType ItemType => itemType;
        public GameObject ItemPrefab => itemPrefab;
        public Sprite ItemIcon => itemIcon;
        public int MaxStackSize => maxStackSize;
        public int GoldValue => goldValue;
        public string ItemDescription => itemDescription;

        public void SetItemID(int newID)
        {
            itemID = newID;
        }

        public abstract void UseItem();
    }
}

using Characters.PlayerSystem;
using Events;
using Items.Core.Interfaces;
using Items.DataPersistence;
using Items.ScriptableObjects;
using SaveLoadSystem;
using UnityEngine;

namespace Items.Components.Pickup
{
    [RequireComponent(typeof(UniqueIdGenerator), typeof(ItemPickupDataHandler))]
    public class ItemPickup : MonoBehaviour, IInteractable, IGrabbable
    {
        [SerializeField] private ItemSO itemData;

        [Header("Prompt")]
        [SerializeField] private string interactionPrompt;

        private string _id;
    
        public ItemSO ItemData => itemData;
        public string ID => _id;

        private void Start()
        {
            _id = GetComponent<UniqueIdGenerator>().ID;
        }

        public string GetInteractionPrompt() => interactionPrompt;

        public void HandleInteract(PlayerInteract playerInteract)
        {
            var inventory = playerInteract.GetComponent<PlayerInventoryHolder>();
            if (inventory is null) return;

            if (!inventory.AddItemToInventory(itemData, 1)) return;
            
            GameEventManager.Instance.MiscEventHandler.InvokeItemCollected(itemData);
            SaveGameManager.Instance.CurrentGameData.collectedItems.Add(_id);
            Destroy(this.gameObject);
        }
    }
}

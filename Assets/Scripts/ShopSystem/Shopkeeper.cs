using Characters.NPC.Friendly;
using Characters.PlayerSystem;
using SaveLoadSystem;
using ShopSystem.Core;
using ShopSystem.DataPersistence;
using ShopSystem.ScriptableObjects;
using UI;
using UnityEngine;

namespace ShopSystem
{
    [RequireComponent(typeof(ShopDataHandler), typeof(UniqueIdGenerator))]
    public class Shopkeeper : FriendlyNPC
    {
        [Header("Shop Data")]
        [SerializeField] private ShopConfigSO shopConfig;
        
        [Header("Shop Container")]
        [SerializeField] private ShopContainer shopContainer;
        
        public ShopContainer ShopContainer => shopContainer;
        
        private void Awake()
        {
            shopContainer = new ShopContainer
                (
                    shopConfig.Items.Count, 
                    shopConfig.MaxGold,
                    shopConfig.PlayerSellMarkUp, 
                    shopConfig.PlayerBuyMarkUp
                );

            foreach (var item in shopConfig.Items)
            {
                shopContainer.AddItemToShopContainer(item.itemData, item.quantity);
            }
        }

        public override string GetInteractionPrompt() => interactionPrompt;

        public override void HandleInteract(PlayerInteract playerInteract)
        {
            playerInteract.gameObject.TryGetComponent<PlayerInventoryHolder>(out var playerInventoryHolder);
            
            if (playerInventoryHolder is not null)
            {
                UIManager.Instance.DisplayShopUI(shopContainer, playerInventoryHolder);
            }
            else
            {
                Debug.LogError("PlayerInventoryHolder is null!");
            }
        }
        
        public void LoadContainer(ShopContainer containerToLoad)
        {
            shopContainer = containerToLoad;
        }
    }
}
using System;
using System.Linq;
using Characters.PlayerSystem;
using Items.Core;
using Items.ScriptableObjects;
using PlayerSystem;
using ShopSystem.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShopSystem.UI
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] private ShopSlotUI shopSlotUI;
        [SerializeField] private Button playerInventoryTab;
        [SerializeField] private Button shopTab;

        [Header("Windows")]
        [SerializeField] private GameObject contentPanel;
        [SerializeField] private GameObject itemPreviewWindow;
        
        [Header("Item Details")] 
        [SerializeField] private Image itemSprite;
        [SerializeField] private GameObject itemDetails;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemValue;
        [SerializeField] private TextMeshProUGUI itemDescription;
        
        [Header("ChooseItemQuantity")]
        [SerializeField] private GameObject confirmItemQuantity;
        [SerializeField] private Slider itemQuantitySlider;
        [SerializeField] private TextMeshProUGUI currentChosenItemQuantity;
        [SerializeField] private TextMeshProUGUI yesButtonHint;
        [SerializeField] private TextMeshProUGUI noButtonHint;
        
        [Header("Footer")]
        [SerializeField] private TextMeshProUGUI buttonHint;
        [SerializeField] private TextMeshProUGUI buttonHintDescription;
        [SerializeField] private TextMeshProUGUI shopGold;
        [SerializeField] private TextMeshProUGUI playerGold;
        
        private ShopContainer _shopContainer;
        private PlayerInventoryHolder _playerInventoryHolder;
        
        private ShopSlotUI _currentSelectedSlotUI;
        private bool _isChoosingItemQuantity;
        private bool _isPlayerSelling;

        private float MarkUp => _isPlayerSelling ? _shopContainer.PlayerSellMarkUp : _shopContainer.PlayerBuyMarkUp;
        
        private void OnEnable()
        {
            itemQuantitySlider.onValueChanged.AddListener(RefreshCurrentChosenItemQuantityText);
            
            playerInventoryTab.onClick.AddListener(OnInventoryTabPressed);
            shopTab.onClick.AddListener(OnShopTabPressed);
        }

        private void OnDisable()
        {
            _isChoosingItemQuantity = false;
            itemQuantitySlider.onValueChanged.RemoveListener(RefreshCurrentChosenItemQuantityText);
            
            playerInventoryTab.onClick.RemoveListener(OnInventoryTabPressed);
            shopTab.onClick.RemoveListener(OnShopTabPressed);
        }

        public void DisplayShopUI(ShopContainer shopContainer, PlayerInventoryHolder playerInventoryHolder)
        {
            _shopContainer = shopContainer;
            _playerInventoryHolder = playerInventoryHolder;

            RefreshDisplay();
        }
        
        public void ClickSlot(ShopSlotUI clickedSlotUI)
        {
            _isChoosingItemQuantity = false;
            
            // Display ChosenBackground for clickedSlotUI
            _currentSelectedSlotUI?.ChosenBackground?.gameObject.SetActive(false);
            _currentSelectedSlotUI = clickedSlotUI;
            _currentSelectedSlotUI.ChosenBackground.gameObject.SetActive(true);
            
            // Display ItemDetails & disable ConfirmItemQuantity
            DisplayItemDetails();
        }

        public void ConfirmTrade()
        {
            if (_currentSelectedSlotUI is null) return;
            
            // Already choosing item quantity
            if (_isChoosingItemQuantity)
            {
                ConfirmChosenQuantity();
                return;
            }

            // First confirm
            if (_currentSelectedSlotUI.AssignedShopSlot.CurrentStackSize > 1)
            {
                DisplayQuantitySelection();
            }
            else
            {
                HandleTransaction(_currentSelectedSlotUI.AssignedShopSlot.ItemData, 1);
            }
        }
        
        public static int GetModifiedItemValue(ItemSO itemData, int quantity, float markUp)
        {
            var baseValue = itemData.GoldValue * quantity;
            return Mathf.FloorToInt(baseValue * markUp);
        }

        private void RefreshCurrentChosenItemQuantityText(float value)
        {
            currentChosenItemQuantity.text = value.ToString();
        }

        private void RefreshDisplay()
        {
            ClearUISlotsInContentPanel();
            itemPreviewWindow.SetActive(false);

            if (_isPlayerSelling)
                DisplayPlayerItems();
            else
                DisplayShopItems();
            
            DisplayFooter();
        }

        private void ClearUISlotsInContentPanel()
        {
            foreach (var item in contentPanel.transform.Cast<Transform>())
            {
                Destroy(item.gameObject);
            }
            
            _currentSelectedSlotUI = null;
        }

        private void DisplayPlayerItems()
        {
            foreach (var item in _playerInventoryHolder.BackpackContainer.GetAllItems())
            {
                if (item.Key.ItemType == ItemType.Quest) return;
                
                var tempShopSlot = new ShopSlot();
                tempShopSlot.AssignItem(item.Key, item.Value);
                
                var shopSlotUIClone = Instantiate(shopSlotUI, contentPanel.transform);
                shopSlotUIClone.Initialize(tempShopSlot, _shopContainer.PlayerSellMarkUp);
            }
        }

        private void DisplayShopItems()
        {
            foreach (var item in _shopContainer.ShopSlots)
            {
                if (item.ItemData is null) return;
                
                var shopSlotUIClone = Instantiate(shopSlotUI, contentPanel.transform);
                shopSlotUIClone.Initialize(item, _shopContainer.PlayerBuyMarkUp);
            }
        }
        
        private void DisplayItemDetails()
        {
            itemPreviewWindow.SetActive(true);
            itemDetails.SetActive(true);
            confirmItemQuantity.SetActive(false);
            
            var assignedShopSlot = _currentSelectedSlotUI.AssignedShopSlot;
            itemSprite.sprite = assignedShopSlot.ItemData.ItemIcon;
            itemSprite.color = Color.white;
            itemName.text = _currentSelectedSlotUI.AssignedShopSlot.CurrentStackSize > 1 ? 
                $"{assignedShopSlot.ItemData.ItemName} ({assignedShopSlot.CurrentStackSize})" : 
                $"{assignedShopSlot.ItemData.ItemName}";
            
            var modifiedItemValue = GetModifiedItemValue(assignedShopSlot.ItemData, 1, MarkUp);
            itemValue.text = $"{modifiedItemValue}";
            itemDescription.text = assignedShopSlot.ItemData.ItemDescription;
        }

        private void DisplayFooter()
        {
            buttonHintDescription.text = _isPlayerSelling ? "Sell" : "Buy";
            shopGold.text = _shopContainer.AvailableGold.ToString();
            playerGold.text = _playerInventoryHolder.Container.Gold.ToString();
        }
        
        private void ConfirmChosenQuantity()
        {
            _isChoosingItemQuantity = false;
            
            itemDetails.SetActive(true);
            confirmItemQuantity.SetActive(false);
                
            var chosenQuantity = (int) itemQuantitySlider.value;
            HandleTransaction(_currentSelectedSlotUI.AssignedShopSlot.ItemData, chosenQuantity);
        }

        private void DisplayQuantitySelection()
        {
            _isChoosingItemQuantity = true;
            
            itemDetails.SetActive(false);
            confirmItemQuantity.SetActive(true);
                
            itemQuantitySlider.maxValue = MaxQuantityCanChoose();
            itemQuantitySlider.value = itemQuantitySlider.maxValue;
        }

        private int MaxQuantityCanChoose()
        {
            var itemData = _currentSelectedSlotUI.AssignedShopSlot.ItemData;
            var stockAvailable = _currentSelectedSlotUI.AssignedShopSlot.CurrentStackSize;
            var goldToCheck = _isPlayerSelling ? _shopContainer.AvailableGold : _playerInventoryHolder.Container.Gold;
            var modifiedItemValue = GetModifiedItemValue(_currentSelectedSlotUI.AssignedShopSlot.ItemData, 1, MarkUp);
            
            var quantityGoldCanBuy = goldToCheck / modifiedItemValue;
            if (_isPlayerSelling) // Player is selling to shop
            {
                // Max = min(stock player has, how much gold shop can pay)
                return Mathf.Min(stockAvailable, quantityGoldCanBuy);
            }

            // Player is buying from shop
            _playerInventoryHolder.BackpackContainer.CheckContainerRemaining(itemData, out var quantityContainerCanReceive);
            return Mathf.Min(stockAvailable, quantityGoldCanBuy, quantityContainerCanReceive);
        }
        
        // TODO: now on player, primary container holds gold not backpack container but backpack container holds items can trade
        // Consider to choose which container holds gold
        private void HandleTransaction(ItemSO itemData, int quantity)
        {
            if (quantity < 1) return;
            
            var modifiedItemValue = GetModifiedItemValue(itemData, quantity, MarkUp);
            // 1. If player is selling...
            if (_isPlayerSelling)
            {
                _shopContainer.PurchaseItem(itemData, quantity);
                _shopContainer.SpendGold(modifiedItemValue);
                
                _playerInventoryHolder.BackpackContainer.RemoveItemFromContainer(itemData, quantity);
                _playerInventoryHolder.Container.ReceiveGold(modifiedItemValue);
            }
            // 2. Else check if player backpack container can add this quantity of item.
            else if (_playerInventoryHolder.BackpackContainer.AddItemToContainer(itemData, quantity))
            {
                _shopContainer.SellItem(itemData, quantity);
                _shopContainer.ReceiveGold(modifiedItemValue);
                
                _playerInventoryHolder.Container.SpendGold(modifiedItemValue);
            }
            
            RefreshDisplay();
        }
        
        private void OnInventoryTabPressed()
        {
            _isPlayerSelling = true;
            RefreshDisplay();
        }

        private void OnShopTabPressed()
        {
            _isPlayerSelling = false;
            RefreshDisplay();
        }
    }
}
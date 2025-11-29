using System;
using ShopSystem.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShopSystem.UI
{
    public class ShopSlotUI : MonoBehaviour
    {
        [SerializeField] private ShopSlot assignedShopSlot;
        
        [SerializeField] private Image chosenBackground;
        [SerializeField] private Image itemSprite;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemValue;
        
        private Button _button;

        public ShopSlot AssignedShopSlot => assignedShopSlot;
        public Image ChosenBackground => chosenBackground;
        public float MarkUp { get; private set; }
        public ShopUI ParentShopUI { get; private set; }
        
        private void Awake()
        {
            chosenBackground.gameObject.SetActive(false);
            
            itemSprite.sprite = null;
            itemSprite.preserveAspect = true;
            itemSprite.color = Color.clear;
            itemName.text = "";
            itemValue.text = "";
            
            _button = GetComponent<Button>();
            _button?.onClick.AddListener(OnSlotUIClick);
            
            ParentShopUI = GetComponentInParent<ShopUI>();
        }

        public void Initialize(ShopSlot shopSlot, float markUp)
        {
            this.assignedShopSlot = shopSlot;
            this.MarkUp = markUp;
            UpdateSlotUI();
        }

        private void UpdateSlotUI()
        {
            if (assignedShopSlot.ItemData is not null)
            {
                itemSprite.sprite = assignedShopSlot.ItemData.ItemIcon;
                itemSprite.color = Color.white;
                itemName.text = assignedShopSlot.CurrentStackSize > 1 ? 
                    $"{assignedShopSlot.ItemData.ItemName} ({assignedShopSlot.CurrentStackSize})" : 
                    $"{assignedShopSlot.ItemData.ItemName}";
                
                var modifiedItemValue = ShopUI.GetModifiedItemValue(assignedShopSlot.ItemData, 1, MarkUp);
                itemValue.text = $"{modifiedItemValue}";
            }
            else
            {
                itemSprite.sprite = null;
                itemSprite.color = Color.clear;
                itemName.text = "";
                itemValue.text = "";
            }
        }
        
        private void OnSlotUIClick()
        {
            ParentShopUI?.ClickSlot(this);
        }
    }
}
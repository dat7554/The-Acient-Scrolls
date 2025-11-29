using Characters.PlayerSystem;
using InventorySystem.Core;
using TMPro;
using UnityEngine;

namespace InventorySystem.UI
{
    public class InventoryUIController : MonoBehaviour
    {
        [Header("Inventory UI")]
        [SerializeField] private GameObject playerStashUI;
        [SerializeField] private HotbarUI hotbarUI;
        [SerializeField] private DynamicInventoryUI backpackUI;
        [Space]
        [SerializeField] private TextMeshProUGUI playerGold;
        [Space]
        [SerializeField] private DynamicInventoryUI chestUI;
        [SerializeField] private CursorSlotUI cursorSlotUI;
    
        [Header("Inventory Holder")]
        [SerializeField] private PlayerInventoryHolder playerInventoryHolder;
    
        public HotbarUI Hotbar => hotbarUI;
        
        private void Awake()
        {
            playerStashUI.gameObject.SetActive(false);
            
            chestUI.gameObject.SetActive(false);
            cursorSlotUI.gameObject.SetActive(false);
        }
        
        public void DisplayInventoryUI()
        {
            playerStashUI.gameObject.SetActive(true);
            backpackUI.UpdateDynamicInventoryUI(playerInventoryHolder.BackpackContainer);
            
            cursorSlotUI.gameObject.SetActive(true);
            
            playerGold.text = playerInventoryHolder.Container.Gold.ToString();
        }

        public void HideInventoryUI()
        {
            playerStashUI.gameObject.SetActive(false);
            backpackUI.MoveItemFromCursorToContainer();
            
            cursorSlotUI.gameObject.SetActive(false);
            chestUI.gameObject.SetActive(false);
        }

        public void DisplayChestUI(InventoryContainer containerToDisplay)
        {
            if (playerStashUI.gameObject.activeInHierarchy == false)
            {
                DisplayInventoryUI();
            }
        
            chestUI.gameObject.SetActive(true);
            chestUI.UpdateDynamicInventoryUI(containerToDisplay);
        }

        public void HideChestUI()
        {
            chestUI.gameObject.SetActive(false);
            
            chestUI.MoveItemFromCursorToContainer();
        }
    }
}

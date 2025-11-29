using Characters.PlayerSystem;
using ShopSystem.Core;
using UnityEngine;

namespace ShopSystem.UI
{
    // TODO: reconsider this class role to be combined to ShopUI
    public class ShopUIController : MonoBehaviour
    {
        [Header("Shop UI")]
        [SerializeField] private ShopUI shopUI;

        private void Awake()
        {
            shopUI.gameObject.SetActive(false);
        }
        
        public void DisplayShopUI(ShopContainer shopContainer, PlayerInventoryHolder playerInventoryHolder)
        {
            shopUI.gameObject.SetActive(true);
            shopUI.DisplayShopUI(shopContainer, playerInventoryHolder);
        }

        public void HideShopUI()
        {
            shopUI.gameObject.SetActive(false);
        }

        public void ConfirmTrade()
        {
            if (shopUI.gameObject.activeInHierarchy)
                shopUI.ConfirmTrade();
        }
    }
}

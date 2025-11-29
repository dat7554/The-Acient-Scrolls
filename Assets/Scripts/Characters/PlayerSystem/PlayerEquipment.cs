using Events;
using Items.Components.Shields;
using Items.Components.Weapons;
using Items.Core;
using Items.DataPersistence;
using Items.ScriptableObjects;
using UnityEngine;

namespace Characters.PlayerSystem
{
    public class PlayerEquipment : MonoBehaviour
    {
        [Header("Current Equipment")]
        [SerializeField] private ItemSO currentEquippedLeftHandItemData;
        [SerializeField] private ItemSO currentEquippedRightHandItemData;
        
        [Header("Arms References")]
        [SerializeField] private Transform leftHandTransformOnArms;
        [SerializeField] private Transform rightHandTransformOnArms;
        
        [Header("Arms Equipment")]
        [SerializeField] private GameObject leftHandItemOnArms;
        [SerializeField] private GameObject rightHandItemOnArms;
        
        [Header("Shadow References")]
        [SerializeField] private Transform leftSheathTransformOnShadow;
        [SerializeField] private Transform rightSheathTransformOnShadow;
        [Space]
        [SerializeField] private Transform leftHandTransformOnShadow;
        [SerializeField] private Transform rightHandTransformOnShadow;
        
        [Header("Shadow Equipment")]
        [SerializeField] private GameObject leftHandItemOnShadow;
        [SerializeField] private GameObject rightHandItemOnShadow;
        
        public MeleeWeapon CurrentRightHandMeleeWeaponComponent { get; private set; }
        public Shield CurrentLeftHandShieldComponent { get; private set; }
        
        public bool HasEquippedWeapon => currentEquippedRightHandItemData is not null;
        public bool HasEquippedShield => currentEquippedLeftHandItemData is not null;
        
        public bool HasDrawnWeapon { get; private set; }
        public bool HasDrawnShield { get; private set; }

        private void OnEnable()
        {
            GameEventManager.Instance.UIEventHandler.EquipmentSlotUnequipped += HandleEquipmentSlotUnequipped;
            
            GameEventManager.Instance.PlayerEventHandler.ShieldDrawn += DrawShieldOnArms;
            GameEventManager.Instance.PlayerEventHandler.ShieldSheath += SheathShieldOnArms;
            
            GameEventManager.Instance.PlayerEventHandler.ShieldDrawn += DrawShieldOnShadow;
            GameEventManager.Instance.PlayerEventHandler.ShieldSheath += SheathShieldOnShadow;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.UIEventHandler.EquipmentSlotUnequipped -= HandleEquipmentSlotUnequipped;
            
            GameEventManager.Instance.PlayerEventHandler.ShieldDrawn -= DrawShieldOnArms;
            GameEventManager.Instance.PlayerEventHandler.ShieldSheath -= SheathShieldOnArms;
            
            GameEventManager.Instance.PlayerEventHandler.ShieldDrawn -= DrawShieldOnShadow;
            GameEventManager.Instance.PlayerEventHandler.ShieldSheath -= SheathShieldOnShadow;
        }

        private void Update()
        {
            UpdateLeftEquipment(currentEquippedLeftHandItemData);
            UpdateRightEquipment(currentEquippedRightHandItemData);
        }

        public void UpdateLeftEquipment(ItemSO leftItem)
        {
            if (leftItem is not null && leftItem.ItemType != ItemType.Shield) return;
            
            bool leftChanged = leftItem != currentEquippedLeftHandItemData;
            currentEquippedLeftHandItemData = leftItem;
            
            if (leftChanged && HasDrawnShield)
            {
                if (leftHandItemOnArms is not null)
                {
                    Destroy(leftHandItemOnArms);
                    Destroy(leftHandItemOnShadow);
                }

                //GameEventManager.Instance.PlayerEventHandler.InvokeEquippedLeftItemChanged();
                GameEventManager.Instance.PlayerEventHandler.InvokeToggleShieldRequested(HasDrawnShield);
                HasDrawnShield = false;
            }
        }
        
        public void UpdateRightEquipment(ItemSO rightItem)
        {
            if (rightItem is not null && rightItem.ItemType != ItemType.Weapon) return;
            
            bool rightChanged = rightItem != currentEquippedRightHandItemData;
            currentEquippedRightHandItemData = rightItem;

            if (rightChanged)
            {
                if (rightItem is not null)
                    ChangeWeaponOnRightHand();
            
                if (HasDrawnWeapon)
                {
                    if (rightHandItemOnArms is not null)
                    {
                        Destroy(rightHandItemOnArms);
                        Destroy(rightHandItemOnShadow);
                    }

                    //GameEventManager.Instance.PlayerEventHandler.InvokeEquippedRightItemChanged();
                    GameEventManager.Instance.PlayerEventHandler.InvokeToggleWeaponRequested(HasDrawnWeapon);
                    HasDrawnWeapon = false;
                }
            }
        }
    
        public void DrawWeaponOnArms()
        {
            rightHandItemOnArms = Instantiate
                (
                    currentEquippedRightHandItemData.ItemPrefab, 
                    rightHandTransformOnArms
                );
            CurrentRightHandMeleeWeaponComponent = rightHandItemOnArms.GetComponent<MeleeWeapon>();
            CurrentRightHandMeleeWeaponComponent.SetVisibility(ItemVisibilityMode.FirstPerson);
            CurrentRightHandMeleeWeaponComponent.OnEquip();
        
            // Mark item as equipped to prevent saving
            var itemPickupHandler = rightHandItemOnArms.GetComponent<ItemPickupDataHandler>();
            if (itemPickupHandler != null)
            {
                itemPickupHandler.MarkAsEquipped();
            }
            
            HasDrawnWeapon = true;
        }
        
        public void DrawShieldOnArms()
        {
            leftHandItemOnArms = Instantiate
            (
                currentEquippedLeftHandItemData.ItemPrefab, 
                leftHandTransformOnArms
            );
            CurrentLeftHandShieldComponent = leftHandItemOnArms.GetComponent<Shield>();
            CurrentLeftHandShieldComponent.SetVisibility(ItemVisibilityMode.FirstPerson);
            CurrentLeftHandShieldComponent.OnEquip();
            
            // Mark item as equipped to prevent saving
            var itemPickupHandler = leftHandItemOnArms.GetComponent<ItemPickupDataHandler>();
            if (itemPickupHandler != null)
            {
                itemPickupHandler.MarkAsEquipped();
            }
        
            HasDrawnShield = true;
        }

        public void SheathWeaponOnArms()
        {
            Destroy(rightHandItemOnArms);
        
            HasDrawnWeapon = false;
        }
        
        public void SheathShieldOnArms()
        {
            Destroy(leftHandItemOnArms);
        
            HasDrawnShield = false;
        }

        public void DrawWeaponOnShadow()
        {
            if (rightHandItemOnShadow == null) return;
            
            SetItemParent(rightHandItemOnShadow, rightHandTransformOnShadow);
            
            var weapon = rightHandItemOnShadow.GetComponent<MeleeWeapon>();
            weapon.SetVisibility(ItemVisibilityMode.ShadowOnly);
            weapon.OnEquip();
            
            // Mark shadow weapon as equipped
            var itemPickupHandler = rightHandItemOnShadow.GetComponent<ItemPickupDataHandler>();
            if (itemPickupHandler != null)
            {
                itemPickupHandler.MarkAsEquipped();
            }
        }
        
        public void DrawShieldOnShadow()
        {
            if (!currentEquippedLeftHandItemData) return;

            leftHandItemOnShadow = Instantiate
            (
                currentEquippedLeftHandItemData.ItemPrefab, 
                leftHandTransformOnShadow
            );
            var shield = leftHandItemOnShadow.GetComponent<Shield>();
            shield.SetVisibility(ItemVisibilityMode.ShadowOnly);
            shield.OnEquip();
            
            // Mark shadow shield as equipped
            var itemPickupHandler = leftHandItemOnShadow.GetComponent<ItemPickupDataHandler>();
            if (itemPickupHandler != null)
            {
                itemPickupHandler.MarkAsEquipped();
            }
        }

        public void SheathWeaponOnShadow()
        {
            if (rightHandItemOnShadow == null) return;
            
            SetItemParent(rightHandItemOnShadow, leftSheathTransformOnShadow);
            
            var weapon = rightHandItemOnShadow.GetComponent<MeleeWeapon>();
            weapon.SetVisibility(ItemVisibilityMode.ShadowOnly);
            weapon.OnEquip();
        }
        
        public void SheathShieldOnShadow()
        {
            Destroy(leftHandItemOnShadow);
            
            // no-op
        }

        private void ChangeWeaponOnRightHand()
        {
            if (rightHandItemOnShadow != null)
            {
                Destroy(rightHandItemOnShadow);
            }
        
            rightHandItemOnShadow = Instantiate
                (
                    currentEquippedRightHandItemData.ItemPrefab, 
                    leftSheathTransformOnShadow
                );
            var weapon = rightHandItemOnShadow.GetComponent<MeleeWeapon>();
            weapon.SetVisibility(ItemVisibilityMode.ShadowOnly);
            weapon.OnEquip();
            
            // Mark shadow weapon as equipped
            var itemPickupHandler = rightHandItemOnShadow.GetComponent<ItemPickupDataHandler>();
            if (itemPickupHandler != null)
            {
                itemPickupHandler.MarkAsEquipped();
            }
        }

        private void ChangeWeaponOnRightSheath()
        {
            // no-op
        }

        private void HandleEquipmentSlotUnequipped(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    UpdateRightEquipment(null);
                    break;
                case ItemType.Shield:
                    UpdateLeftEquipment(null);
                    break;
            }
        }

        private void SetItemParent(GameObject item, Transform parent)
        {
            item.transform.SetParent(parent);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
    }
}

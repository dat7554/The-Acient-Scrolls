using Characters;
using Items.Core.Interfaces;
using Items.ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering;

namespace Items.Components.Weapons
{
    public class Weapon : MonoBehaviour, IItemStateHandler
    {
        [SerializeField] protected WeaponItemSO weaponData;
        [SerializeField] protected Collider worldCollider;
        
        protected Character Owner;
        protected Rigidbody RB;
  
        private MeshRenderer _renderer;

        protected virtual void Awake()
        {
            RB = GetComponent<Rigidbody>();
            
            _renderer = GetComponentInChildren<MeshRenderer>();
        }

        public void OnEquip() => SetWorldState(false);

        public void OnDrop()
        {
            SetWorldState(true);
            SetVisibility(ItemVisibilityMode.World);
        }
        
        public void SetVisibility(ItemVisibilityMode mode)
        {
            _renderer.shadowCastingMode = mode switch
            {
                ItemVisibilityMode.World => ShadowCastingMode.On,
                ItemVisibilityMode.FirstPerson => ShadowCastingMode.Off,
                ItemVisibilityMode.ShadowOnly => ShadowCastingMode.ShadowsOnly,
                _ => ShadowCastingMode.On
            };
        }

        private void SetWorldState(bool isInWorld)
        {
            if (isInWorld)
            {
                _renderer.shadowCastingMode = ShadowCastingMode.On;
                
                // Drop item
                transform.SetParent(null, true);
            }
            
            worldCollider.enabled = isInWorld;
            
            RB.isKinematic = !isInWorld;
            RB.useGravity  = isInWorld;
            RB.collisionDetectionMode = isInWorld 
                ? CollisionDetectionMode.ContinuousDynamic : CollisionDetectionMode.Discrete;
        }
    }
}

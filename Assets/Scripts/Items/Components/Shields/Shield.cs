using Characters;
using Items.Components.Weapons;
using Items.Core.Interfaces;
using Items.ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering;

namespace Items.Components.Shields
{
    public class Shield : MonoBehaviour, IItemStateHandler
    {
        [SerializeField] private ShieldItemSO shieldData;
        
        [Header("Mesh Renderers")]
        [SerializeField] private MeshRenderer bodyRenderer;
        [SerializeField] private MeshRenderer handleRenderer;
        
        [Header("Collider")]
        [SerializeField] private ShieldCollider shieldCollider;
        
        private Rigidbody _rigidbody;
        private Collider _worldCollider;
        
        public Character Owner { get; private set; }
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _worldCollider = GetComponent<Collider>();
        }

        private void Start()
        {
            Owner = GetComponentInParent<Character>();
            
            shieldCollider.Initialize(Owner);
        }
        
        public void OnEquip() => SetWorldState(false);

        public void OnDrop()
        {
            SetWorldState(true);
            SetVisibility(ItemVisibilityMode.World);
        }
        
        public void SetVisibility(ItemVisibilityMode mode)
        {
            bodyRenderer.shadowCastingMode = mode switch
            {
                ItemVisibilityMode.World => ShadowCastingMode.On,
                ItemVisibilityMode.FirstPerson => ShadowCastingMode.Off,
                ItemVisibilityMode.ShadowOnly => ShadowCastingMode.ShadowsOnly,
                _ => ShadowCastingMode.On
            };
            
            handleRenderer.shadowCastingMode = mode switch
            {
                ItemVisibilityMode.World => ShadowCastingMode.On,
                ItemVisibilityMode.FirstPerson => ShadowCastingMode.Off,
                ItemVisibilityMode.ShadowOnly => ShadowCastingMode.ShadowsOnly,
                _ => ShadowCastingMode.On
            };
        }
        
        public void StartBlock()
        {
            shieldCollider.StartBlock();
        }

        public void EndBlock()
        {
            shieldCollider.EndBlock();
        }
        
        private void SetWorldState(bool isInWorld)
        {
            if (isInWorld)
            {
                bodyRenderer.shadowCastingMode = ShadowCastingMode.On;
                handleRenderer.shadowCastingMode = ShadowCastingMode.On;
                
                // Drop item
                transform.SetParent(null, true);
            }
            
            _worldCollider.enabled = isInWorld;
            
            _rigidbody.isKinematic = !isInWorld;
            _rigidbody.useGravity  = isInWorld;
            _rigidbody.collisionDetectionMode = isInWorld 
                ? CollisionDetectionMode.ContinuousDynamic : CollisionDetectionMode.Discrete;
        }
    }
}
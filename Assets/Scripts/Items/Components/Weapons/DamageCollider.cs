using System.Collections.Generic;
using Characters;
using Interfaces;
using Items.Components.Shields;
using UnityEngine;

namespace Items.Components.Weapons
{
    public class DamageCollider : MonoBehaviour
    {
        [SerializeField] private List<GameObject> damagedObjects = new List<GameObject>();
        [SerializeField] private bool canDealDamage;
    
        private float _damage;
        private Collider _collider;
        private Character _owner;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
        }

        public void Initialize(float damage, Character owner)
        {
            _damage = damage;
            _owner = owner;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!canDealDamage) return;
        
            // Shield check
            if (other.CompareTag("Shield"))
            {
                canDealDamage = false;
                _owner.HandleAttackHit();
                return;
            }
            
            // Prevent double hits
            if (damagedObjects.Contains(other.gameObject)) return;
            
            // Deal damage
            IDamageable damageableTarget = other.gameObject.GetComponentInParent<IDamageable>();
            if (damageableTarget is null) return;
            
            // Ignore same faction
            Character character = other.GetComponentInParent<Character>();
            if (character?.Faction == _owner.Faction)
            {
                EndDealDamage();
                return;
            }
            
            _owner.HandleAttackHit();

            float? hitAngle = null;
            if (character)
            {
                hitAngle = Vector3.SignedAngle(_owner.ForwardTransform, character.ForwardTransform, Vector3.up);
            }

            damageableTarget.TakeDamage(_damage, hitAngle);
            damagedObjects.Add(other.gameObject);
        }

        public void StartDealDamage()
        {
            canDealDamage = true;
            _collider.enabled = true;
        }

        public void EndDealDamage()
        {
            canDealDamage = false;
            _collider.enabled = false;
        
            damagedObjects.Clear();
        }
    }
}
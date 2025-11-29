using System;
using System.Collections;
using Characters;
using UnityEngine;
using UnityEngine.Pool;

namespace Items.Components.Weapons
{
    public class Arrow : Weapon
    {
        [SerializeField] private DamageCollider damageCollider;
        [SerializeField] private float arrowVelocity = 100f;
        [SerializeField] private float timeoutDelay = 10f;
        
        [Header("VFX")]
        [SerializeField] private TrailRenderer trailRenderer;
        
        private IObjectPool<Arrow> _pool;
        private Coroutine _deactivateRoutine;
        
        public IObjectPool<Arrow> Pool { set => _pool = value; } // Set a reference to its ObjectPool

        protected override void Awake()
        {
            base.Awake();
            
            if (trailRenderer == null)
                trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        public void Launch(Transform arrowPlacementPosition, Character owner)
        {
            trailRenderer.enabled = true;
            
            damageCollider.Initialize(weaponData.Damage, owner);
            RB.AddForce(arrowPlacementPosition.forward * arrowVelocity, ForceMode.Impulse);
            StartDamage();
            _deactivateRoutine = StartCoroutine(DeactivateRoutine(timeoutDelay));
        }

        private void OnCollisionEnter()
        {
            if (_deactivateRoutine != null)
            {
                StopCoroutine(_deactivateRoutine);
                _deactivateRoutine = null;
            }
            
            StartCoroutine(DeactivateRoutine(1f));
        }
        
        private void StartDamage()
        {
            damageCollider.StartDealDamage();
        }
        
        private void EndDamage()
        {
            damageCollider.EndDealDamage();
        }
        
        private IEnumerator DeactivateRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            DeactivateImmediate();
        }

        private void DeactivateImmediate()
        {
            trailRenderer.enabled = false;
            
            EndDamage();
            
            RB.isKinematic = false;
            RB.useGravity = true;
            RB.linearVelocity = Vector3.zero;
            RB.angularVelocity = Vector3.zero;
            
            transform.SetParent(null, true);
            
            _pool.Release(this);
        }
    }
}

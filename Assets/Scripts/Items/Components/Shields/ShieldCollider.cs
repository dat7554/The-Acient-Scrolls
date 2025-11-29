using Characters;
using Items.Components.Weapons;
using UnityEngine;

namespace Items.Components.Shields
{
    public class ShieldCollider : MonoBehaviour
    {
        [SerializeField] private bool canBlockDamage;
        
        private Collider _collider;
        private Character _owner;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
        }
        
        public void Initialize(Character owner)
        {
            _owner = owner;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!canBlockDamage) return;
            
            Debug.Log("Block a hit");
            _owner.HandleSuccessfulBlock();
        }

        public void StartBlock()
        {
            canBlockDamage = true;
            _collider.enabled = true;
        }

        public void EndBlock()
        {
            canBlockDamage = false;
            _collider.enabled = false;
        }
    }
}
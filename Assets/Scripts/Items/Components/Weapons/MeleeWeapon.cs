using Characters;
using UnityEngine;

namespace Items.Components.Weapons
{
    public class MeleeWeapon : Weapon
    {
        [Header("Damage Colliders")]
        [SerializeField] private DamageCollider bladeCollider;
        [SerializeField] private DamageCollider buttCollider;

        private const float ButtDamageMultiplier = 0.25f;
        
        private ParticleSystem _particleSystem;

        protected override void Awake()
        {
            base.Awake();
            
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        protected void Start()
        {
            Owner = GetComponentInParent<Character>();
            
            _particleSystem.Stop();
        
            bladeCollider.Initialize(weaponData.Damage, Owner);
            buttCollider.Initialize(weaponData.Damage * ButtDamageMultiplier, Owner);
        }

        public void PlayParticles()
        {
            _particleSystem.Play();
        }

        public void StopParticles()
        {
            _particleSystem.Stop();
        }

        public void StartBladeDamage()
        {
            bladeCollider.StartDealDamage();
        }

        public void StartButtDamage()
        {
            buttCollider.StartDealDamage();
        }

        public void EndBladeDamage()
        {
            bladeCollider.EndDealDamage();
        }
    
        public void EndButtDamage()
        {
            buttCollider.EndDealDamage();
        }
    }
}

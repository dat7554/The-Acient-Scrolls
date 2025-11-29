using System;

namespace Characters.NPC.Enemy
{
    public class EnemyWarriorAnimatorEventReceiver : EnemyNPCAnimatorEventReceiver
    {
        private EnemyWarrior _enemyWarrior;

        private void Awake()
        {
            _enemyWarrior = GetComponent<EnemyWarrior>();
        }

        public void PlayWeaponTrail()
        {
            _enemyWarrior.RightMeleeWeapon.PlayParticles();
        }
        
        public void StopWeaponTrail()
        {
            _enemyWarrior.RightMeleeWeapon.StopParticles();
        }
        
        public override void OnDamageStart(int hitType)
        {
            _enemyWarrior.PlayWeaponWhooshSoundFX();
            
            if ((AttackHitType) hitType == AttackHitType.Blade)
            {
                _enemyWarrior.RightMeleeWeapon.StartBladeDamage();
            }
            else if ((AttackHitType) hitType == AttackHitType.Butt)
            {
                _enemyWarrior.RightMeleeWeapon.StartButtDamage();
            }
        }
        
        public override void OnDamageEnd(int hitType)
        {
            if ((AttackHitType) hitType == AttackHitType.Blade)
            {
                _enemyWarrior.RightMeleeWeapon.EndBladeDamage();
            }
            else if ((AttackHitType) hitType == AttackHitType.Butt)
            {
                _enemyWarrior.RightMeleeWeapon.EndButtDamage();
            }
        }
    }
}

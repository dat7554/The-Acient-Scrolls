using System.Collections;
using Items.Components.Shields;
using Items.Components.Weapons;
using UnityEngine;

namespace Characters.NPC.Enemy
{
    public class EnemyWarrior : EnemyNPC
    {
        private static readonly int TriggerButtAttack = Animator.StringToHash("TriggerButtAttack");
        private static readonly int TriggerBladeAttack = Animator.StringToHash("TriggerBladeAttack");
        
        [Header("Equipment")]
        [SerializeField] private Shield leftShield;
        [SerializeField] private MeleeWeapon rightMeleeWeapon;

        public MeleeWeapon RightMeleeWeapon => rightMeleeWeapon;
        
        private void Start()
        {
            leftShield?.OnEquip();
            rightMeleeWeapon?.OnEquip();
        }

        public override int ChooseAttackAnimation(bool isTargetTooClose)
        {
            return isTargetTooClose ? TriggerButtAttack : TriggerBladeAttack;
        }

        public override void HandleAttackHit()
        {
            // TODO: consider to be stunt when add parry system
            
            _animator?.SetTrigger(TriggerCombatIdle);
            StartCoroutine(ResetTriggerNextFrame());
        }
        
        private IEnumerator ResetTriggerNextFrame()
        {
            yield return null;
            _animator.ResetTrigger(TriggerCombatIdle);
        }
        
        public void PlayWeaponWhooshSoundFX()
        {
            _enemyNpcSoundFX.PlayWeaponWhooshSoundFX();
        }
    }
}

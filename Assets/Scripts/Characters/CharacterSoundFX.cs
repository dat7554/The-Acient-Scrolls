using Audio;
using UnityEngine;

namespace Characters
{
    public class CharacterSoundFX : SoundFX
    {
        [Header("MeleeWeapon Whooshes SFX")]
        [SerializeField] private AudioClip[] weaponWhooshes;
        
        [Header("Damage Grunts SFX")]
        [SerializeField] private AudioClip[] damageGrunts;
        
        [Header("Block Impacts SFX")]
        [SerializeField] private AudioClip[] blockImpacts;
        
        public void PlayWeaponWhooshSoundFX()
        {
            PlaySoundFX(AudioManager.Instance.GetRandomElement(weaponWhooshes), volume: 0.25f);
        }

        public void PlayDamageGruntSoundFX()
        {
            PlaySoundFX(AudioManager.Instance.GetRandomElement(damageGrunts), volume: 0.5f);
        }

        public void PlayBlockImpactSoundFX()
        {
            PlaySoundFX(AudioManager.Instance.GetRandomElement(blockImpacts), volume: 0.75f);
        }
    }
}
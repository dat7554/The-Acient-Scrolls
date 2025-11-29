using Audio;
using Characters.PlayerSystem.Enums;
using Items.Core;
using UnityEngine;

namespace Characters.PlayerSystem
{
    public class PlayerSoundFX : CharacterSoundFX
    {
        [Header("Footsteps SFX")]
        [SerializeField] private AudioClip[] footstepSounds;
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip landSound;
        
        [Header("Consume SFX")]
        [SerializeField] private AudioClip drinkSound;
        [SerializeField] private AudioClip eatSound;
        
        [Header("Breaths SFX")]
        [SerializeField] private AudioSource breathAudioSource;
        [SerializeField] private AudioClip breathSound;
        [SerializeField] private float staminaModerateThreshold = 30f;

        private BreathState _currentBreathState = BreathState.Normal;

        public void PlayWalkOrSprintSoundFX(float volume = 1f)
        {
            PlaySoundFX(AudioManager.Instance.GetRandomElement(footstepSounds), volume);
        }

        public void PlayJumpSoundFX()
        {
            PlaySoundFX(jumpSound);
        }

        public void PlayLandSoundFX()
        {
            PlaySoundFX(landSound);
        }

        public void PlayConsumeSoundFX(ConsumeType consumeType)
        {
            if (consumeType == ConsumeType.Eat)
            {
                PlaySoundFX(eatSound, volume: 0.75f);
            }
            else if (consumeType == ConsumeType.Drink)
            {
                PlaySoundFX(drinkSound, volume: 0.75f);
            }
        }

        public void PlayBreathSoundFX(float staminaPercent)
        {
            var newState = staminaPercent <= staminaModerateThreshold ? BreathState.Moderate : BreathState.Normal;

            if (breathAudioSource.isPlaying || newState == BreathState.Normal) return;
            
            _currentBreathState = newState;
            breathAudioSource.clip = GetAudioClipBaseOnState(_currentBreathState);
            breathAudioSource.Play();
        }

        private AudioClip GetAudioClipBaseOnState(BreathState state)
        {
            return state switch
            {
                BreathState.Normal => null,
                BreathState.Moderate => breathSound,
                _ => null
            };
        }
    }
}
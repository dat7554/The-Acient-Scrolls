using UnityEngine;

namespace Audio
{
    public class ChestSoundFX : SoundFX
    {
        [Header("Open Chest SFX")]
        [SerializeField] private AudioClip[] openChests;

        private void Awake()
        {
            if (audioSource is null)
                audioSource = GetComponent<AudioSource>();
        }
        
        public void PlayOpenChestSoundFX()
        {
            PlaySoundFX(AudioManager.Instance.GetRandomElement(openChests), volume: 0.25f);
        }
    }
}

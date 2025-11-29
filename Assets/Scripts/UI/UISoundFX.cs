using System.Collections;
using Audio;
using Events;
using UnityEngine;

namespace UI
{
    public class UISoundFX : SoundFX
    {
        [Header("Hover SFX")]
        [SerializeField] private AudioClip hoverSound;
        
        private AudioClip _lastPlayedMusic;
        
        private void Awake()
        {
            audioSource ??= GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            GameEventManager.Instance.UIEventHandler.ButtonSelected += PlayHoverSound;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.UIEventHandler.ButtonSelected -= PlayHoverSound;
        }
        
        public void PlayMusicSoundFX()
        {
            AudioClip newClip;
            var musics = AudioManager.Instance.menuMusicSFXs;
            
            do
            {
                newClip = AudioManager.Instance.GetRandomElement(musics);
            }
            while (newClip == _lastPlayedMusic && musics.Length > 0);
            
            _lastPlayedMusic = newClip;
            Play(newClip);
        }

        private void PlayHoverSound()
        {
            PlaySoundFX(hoverSound, volume: 0.35f, randomizePitch: false);
        }
        
        private void Play(AudioClip clip, bool loop = true)
        {
            audioSource.clip = clip;
            audioSource.Play();

            if (loop)
                StartCoroutine(WaitForMusicToEndRoutine());
        }

        private IEnumerator WaitForMusicToEndRoutine()
        {
            yield return new WaitUntil(() => !audioSource.isPlaying);
            PlayMusicSoundFX();
        }
    }
}

using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
    public class SoundFX : MonoBehaviour
    {
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected bool ignoreListenerPause = false;

        protected virtual void Start()
        {
            audioSource.ignoreListenerPause = ignoreListenerPause;
        }

        public void PlaySoundFX(AudioClip audioClip, float volume = 1f, bool randomizePitch = true, float pitchOffset = 0.1f)
        {
            audioSource.pitch = 1f;
            
            if (randomizePitch)
                audioSource.pitch += Random.Range(-pitchOffset, pitchOffset);
            
            audioSource.PlayOneShot(audioClip, volume);
        }
    }
}
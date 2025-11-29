using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance {get; private set;}
        
        [Header("Physical Damage SFX")]
        public AudioClip[] physicalDamageSFX;
        
        [Header("Menu Music SFX")]
        public AudioClip[] menuMusicSFXs;
        
        [Header("Death SFX")]
        public AudioClip deathSFX;
        
        private void Awake()
        {
            if (Instance is null) 
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public T GetRandomElement<T>(IList<T> list)
        {
            if (list is null || list.Count == 0)
                return default;
            
            int randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
    }
}
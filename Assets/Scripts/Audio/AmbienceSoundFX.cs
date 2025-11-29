using System;
using UnityEngine;

namespace Audio
{
    public class AmbienceSoundFX : SoundFX
    {
        [Header("Ambience SFX")]
        [SerializeField] private AudioClip ambience;
        [SerializeField] private Transform playerCharacter;
        [SerializeField] private Collider zoneCollider;
        [SerializeField] private float fadeDistance = 10f;
        
        private Vector3 _defaultPosition;

        protected override void Start()
        {
            base.Start();
            
            _defaultPosition = audioSource.transform.position;
            
            audioSource.clip = ambience;
            audioSource.loop = true;
            audioSource.Play();
        }

        private void FixedUpdate()
        {
            // If playerCharacter inside zone...
            if (zoneCollider.bounds.Contains(playerCharacter.position))
            {
                audioSource.spatialBlend = 0f;
                audioSource.transform.position = _defaultPosition;
                
                if (!audioSource.isPlaying)
                    audioSource.Play();
                
                return;
            }
            
            // Else if playerCharacter outside.
            Vector3 closestPointOnEdge = zoneCollider.ClosestPoint(playerCharacter.position);
            float playerDistanceToEdge = Vector3.Distance(playerCharacter.position, closestPointOnEdge);
            
            if (playerDistanceToEdge > fadeDistance) return;
            
            audioSource.spatialBlend = 1f;
            audioSource.transform.position = closestPointOnEdge;
        }
    }
}

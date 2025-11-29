using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Camera
{
    public class CameraMotionController : MonoBehaviour
    {
        [SerializeField] private NoiseSettings walkAndSprintNoise;
        [SerializeField] private NoiseSettings shakeNoise;
        
        private CinemachineBasicMultiChannelPerlin _perlin;
        private Coroutine _temporaryNoiseCoroutine;
        private bool _isTemporaryNoiseActive;

        public NoiseSettings ShakeNoise => shakeNoise;

        private void Awake()
        {
            _perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void EnableHeadBob(bool enable, float amplitude = 1f, float frequency = 1f)
        {
            if (_isTemporaryNoiseActive) return;
            _perlin.AmplitudeGain = enable ? amplitude : 0f;
            _perlin.FrequencyGain = enable ? frequency : 0f;
        }

        public void PlayTemporaryNoise(NoiseSettings noiseProfile, float duration = 0.25f, float amplitude = 0.65f, float frequency = 0.1f)
        {
            if (_temporaryNoiseCoroutine is not null) StopCoroutine(_temporaryNoiseCoroutine);
            _temporaryNoiseCoroutine = StartCoroutine(TemporaryNoiseRoutine(noiseProfile, duration, amplitude, frequency));
        }

        private IEnumerator TemporaryNoiseRoutine(NoiseSettings noiseProfile, float duration, float amplitude, float frequency)
        {
            _isTemporaryNoiseActive = true;
            _perlin.NoiseProfile = noiseProfile;

            yield return null;
            
            _perlin.AmplitudeGain = amplitude;
            _perlin.FrequencyGain = frequency;
            
            yield return new WaitForSeconds(duration);
            
            _perlin.NoiseProfile = walkAndSprintNoise;
            _temporaryNoiseCoroutine = null;
            _isTemporaryNoiseActive = false;
        }
    }
}

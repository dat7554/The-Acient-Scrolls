using Interfaces;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Settings.UI
{
    public class SoundContainer : MonoBehaviour, IActivatableUI, IConfigurableUI
    {
        [Header("References")]
        [SerializeField] private SettingsMenu settingsMenu;
        [Space]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider effectsSlider;
        [SerializeField] private Slider musicSlider;
        [Space]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;

        private const string MasterVolume = "MasterVolume";
        private const string EffectsVolume = "EffectsVolume";
        private const string MusicVolume = "MusicVolume";

        public void Activate()
        {
            gameObject.SetActive(true);

            LoadSoundSettings();
        }

        public void Deactivate()
        {
            LoadSoundSettings();
            
            gameObject.SetActive(false);
        }
        
        public void OnApplyButtonClick()
        {
            settingsMenu.SaveSoundSettings
                (
                    masterSlider.value, 
                    effectsSlider.value, 
                    musicSlider.value
                );
        }

        public void OnResetButtonClick()
        {
            masterSlider.value = masterSlider.maxValue;
            effectsSlider.value = effectsSlider.maxValue;
            musicSlider.value = musicSlider.maxValue;
        }

        public void SetMasterVolume(float value)
        {
            audioMixer.SetFloat(MasterVolume, Mathf.Log10(value) * 20);
        }

        public void SetEffectsVolume(float value)
        {
            audioMixer.SetFloat(EffectsVolume, Mathf.Log10(value) * 20);
        }

        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat(MusicVolume, Mathf.Log10(value) * 20);
        }

        private void LoadSoundSettings()
        {
            SettingsData settingsData = settingsMenu.LoadSettings();
            masterSlider.value = settingsData.masterVolume;
            effectsSlider.value = settingsData.effectsVolume;
            musicSlider.value = settingsData.musicVolume;
        }
    }
}

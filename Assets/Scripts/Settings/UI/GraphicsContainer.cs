using System;
using System.Collections.Generic;
using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Settings.UI
{
    public class GraphicsContainer : MonoBehaviour, IActivatableUI, IConfigurableUI
    {
        [Header("References")]
        [SerializeField] private SettingsMenu settingsMenu;
        [Space]
        [SerializeField] private VolumeProfile volumeProfile;
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private Toggle fullScreenToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [Space]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;

        private Resolution[] _resolutions;
        
        private void Awake()
        {
            // Populate resolution dropdown
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            
            List<string> options = new List<string>();

            foreach (var resolution in _resolutions)
            {
                string option = resolution.width + " x " + resolution.height;
                options.Add(option);
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.RefreshShownValue();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            
            LoadGraphicsSettings();
        }

        public void Deactivate()
        {
            LoadGraphicsSettings();
            
            gameObject.SetActive(false);
        }

        public void OnApplyButtonClick()
        {
            settingsMenu.SaveGraphicsSettings
            (
                brightnessSlider.value,
                fullScreenToggle.isOn,
                _resolutions[resolutionDropdown.value].width,
                _resolutions[resolutionDropdown.value].height
            );
        }

        public void OnResetButtonClick()
        {
            brightnessSlider.value = 0.2f;
            fullScreenToggle.isOn = true;
            resolutionDropdown.value = _resolutions.Length - 1;
        }

        public void SetBrightness(float value)
        {
            if (volumeProfile.TryGet(out ColorAdjustments colorAdjustments))
            {
                colorAdjustments.postExposure.Override(value);
            }
        }

        public void SetFullScreen(bool value)
        {
            Screen.fullScreen = value;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        
        private void LoadGraphicsSettings()
        {
            SettingsData settingsData = settingsMenu.LoadSettings();
            brightnessSlider.value = settingsData.brightness;
            fullScreenToggle.isOn = settingsData.fullScreen;
            
            int currentOptionIndex = _resolutions.Length - 1;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                if (_resolutions[i].width == settingsData.resolutionWidth 
                    && _resolutions[i].height == settingsData.resolutionHeight)
                {
                    currentOptionIndex = i;
                }
            }
            
            resolutionDropdown.value = currentOptionIndex;
            resolutionDropdown.RefreshShownValue();
        }
    }
}

using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Settings.UI
{
    public class ControlContainer : MonoBehaviour, IActivatableUI, IConfigurableUI
    {
        [Header("References")]
        [SerializeField] private SettingsMenu settingsMenu;
        [Space]
        [SerializeField] private Slider lookSensitivitySlider;
        [SerializeField] private Toggle invertYToggle;
        [Space]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;
        
        public void Activate()
        {
            gameObject.SetActive(true);

            LoadControlSettings();
        }

        public void Deactivate()
        {
            LoadControlSettings();
            
            gameObject.SetActive(false);
        }

        public void OnApplyButtonClick()
        {
            settingsMenu.SaveControlSettings
            (
                lookSensitivitySlider.value,
                invertYToggle.isOn
            );
        }

        public void OnResetButtonClick()
        {
            lookSensitivitySlider.value = 0.2f;
            invertYToggle.isOn = false;
        }
        
        private void LoadControlSettings()
        {
            SettingsData settingsData = settingsMenu.LoadSettings();
            lookSensitivitySlider.value = settingsData.lookSensitivity;
            invertYToggle.isOn = settingsData.invertY;
        }
    }
}

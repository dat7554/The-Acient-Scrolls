using System.IO;
using Events;
using Interfaces;
using Settings.UI;
using UnityEngine;

namespace Settings
{
    public class SettingsMenu : MonoBehaviour, IActivatableUI
    {
        [SerializeField] private ControlContainer controlContainer;
        [SerializeField] private GraphicsContainer graphicsContainer;
        [SerializeField] private SoundContainer soundContainer;

        [Header("Save File Storage Config")]
        [SerializeField] private string saveDirectory = "Settings";
        [SerializeField] private string fileName = "UserSettings";
        
        private ISettingsDataHandler _settingsDataHandler;
        private SettingsData _settingsData = new ();
        
        private void Awake()
        {
            var fullPath = Path.Combine(Application.persistentDataPath, saveDirectory);
            _settingsDataHandler = new FileSettingsDataHandler(fullPath, fileName);
            
            LoadSettings();
        }
        
        public void Activate()
        {
            gameObject.SetActive(true);
            DeactivateContainers();
        }

        public void Deactivate()
        {
            DeactivateContainers();
            gameObject.SetActive(false);
        }

        public void OnGameplayButtonClick()
        {
            controlContainer.Activate();
            graphicsContainer.Deactivate();
            soundContainer.Deactivate();
        }

        public void OnGraphicsButtonClick()
        {
            controlContainer.Deactivate();
            graphicsContainer.Activate();
            soundContainer.Deactivate();
        }

        public void OnSoundButtonClick()
        {
            controlContainer.Deactivate();
            graphicsContainer.Deactivate();
            soundContainer.Activate();
        }
        
        public void SaveControlSettings(float lookSensitivity, bool invertY)
        {
            _settingsData.lookSensitivity = lookSensitivity;
            _settingsData.invertY = invertY;
            
            _settingsDataHandler.Save(_settingsData);
        }

        public void SaveGraphicsSettings(float brightness, bool fullScreen, int resolutionWidth, int resolutionHeight)
        {
            _settingsData.brightness = brightness;
            _settingsData.fullScreen = fullScreen;
            _settingsData.resolutionWidth = resolutionWidth;
            _settingsData.resolutionHeight = resolutionHeight;
            
            _settingsDataHandler.Save(_settingsData);
        }

        public void SaveSoundSettings(float masterVolume, float effectsVolume, float musicVolume)
        {
            _settingsData.masterVolume = masterVolume;
            _settingsData.effectsVolume = effectsVolume;
            _settingsData.musicVolume = musicVolume;
            
            _settingsDataHandler.Save(_settingsData);
        }

        public SettingsData LoadSettings()
        {
            _settingsData = _settingsDataHandler.Load();

            if (_settingsData == null)
            {
                _settingsData = new SettingsData();
                
                // Create the default settings data
                SaveControlSettings(0.2f, false);
                SaveGraphicsSettings(0.2f, true, Screen.currentResolution.width, Screen.currentResolution.height);
                SaveSoundSettings(1f, 1f, 1f);
            }
            
            GameEventManager.Instance.SettingEventHandler.InvokeLookSettingsChanged
                (
                    _settingsData.lookSensitivity, 
                    _settingsData.invertY
                );
            
            return _settingsData;
        }

        private void DeactivateContainers()
        {
            controlContainer.Deactivate();
            graphicsContainer.Deactivate();
            soundContainer.Deactivate();
        }
    }
}

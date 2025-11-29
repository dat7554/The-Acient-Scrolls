using System.Collections.Generic;
using SaveLoadSystem;
using SaveLoadSystem.UI;
using Settings;
using UnityEngine;

namespace UI
{
    public class MainMenuUIController : MonoBehaviour
    {
        [Header("UI Menus")]
        [SerializeField] private GameObject mainMenuUI;
        [Space]
        [SerializeField] private GameObject loadMenuUI;
        [SerializeField] private SaveSlotUI[] saveSlotUIs;
        [Space]
        [SerializeField] private SettingsMenu settingsMenu;
        [Space]
        [SerializeField] private GameObject confirmationUI;
        
        private UISoundFX _uiSoundFX;
        
        private void Awake()
        {
            saveSlotUIs ??= GetComponentsInChildren<SaveSlotUI>();
            _uiSoundFX = GetComponent<UISoundFX>();
            
            mainMenuUI.SetActive(true);
            loadMenuUI.SetActive(false);
            confirmationUI.SetActive(false);
        }

        private void Start()
        {
            // Let file handler and containers in settings to load
            settingsMenu.Activate();
            settingsMenu.Deactivate();
            
            _uiSoundFX.PlayMusicSoundFX();
        }

        public void OnNewGameButtonClicked()
        {
            GameManager.Instance.LoadGame();
        }
        
        public void ActivateLoadMenuUI()
        {
            loadMenuUI.SetActive(true);
            LoadAndDisplaySaveSlotUIs();
        }

        public void DeactivateLoadMenuUI()
        {
            loadMenuUI.SetActive(false);
        }

        public void ActivateSettingsUI()
        {
            mainMenuUI.SetActive(false);
            settingsMenu.Activate();
        }

        public void DeactivateSettingsUI()
        {
            settingsMenu.Deactivate();
            mainMenuUI.SetActive(true);
        }

        public void ActivateQuitConfirmationUI()
        {
            confirmationUI.SetActive(true);
        }

        public void OnConfirmQuitButtonClicked()
        {
            GameManager.Instance.QuitToDesktop();
        }

        public void DeactivateQuitConfirmationUI()
        {
            confirmationUI.SetActive(false);
        }
        
        public void OnSaveSlotUIClicked(SaveSlotUI saveSlotUI)
        {
            SaveGameManager.Instance.ChangeProfileId(saveSlotUI.GetProfileId());
            GameManager.Instance.LoadGameFromSave();
        }
        
        private void LoadAndDisplaySaveSlotUIs()
        {
            Dictionary<string, GameData> profileData = SaveGameManager.Instance.GetAllGameDataProfiles();

            foreach (var saveSlotUI in saveSlotUIs)
            {
                profileData.TryGetValue(saveSlotUI.GetProfileId(), out var gameData);
                saveSlotUI.SetGameData(gameData);

                bool isInteractable = gameData is not null;
                saveSlotUI.SetInteractable(isInteractable);
            }
        }
    }
}

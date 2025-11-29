using System.Collections.Generic;
using Events;
using SaveLoadSystem;
using SaveLoadSystem.UI;
using TMPro;
using UnityEngine;

namespace PauseMenu
{
    public class SaveLoadMenuUIController : MonoBehaviour
    {
        [Header("Navigation")]
        [SerializeField] private GameObject confirmationPopupUI;
        
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI saveLoadLabel;
        
        private SaveSlotUI[] _saveSlotUIs;
        private bool _isLoadingGameData;

        public bool IsConfirmationPopupUIActive => confirmationPopupUI.gameObject.activeInHierarchy;

        private void Awake()
        {
            _saveSlotUIs = GetComponentsInChildren<SaveSlotUI>();
        }

        #region Public Methods
        
        public void OnSaveSlotUIClicked(SaveSlotUI saveSlotUI)
        {
            // Case: load game
            if (_isLoadingGameData)
            {
                this.gameObject.SetActive(false);
                GameEventManager.Instance.InputEventHandler.InvokeCloseActiveUI();
                
                SaveGameManager.Instance.ChangeProfileId(saveSlotUI.GetProfileId());
                GameManager.Instance.LoadGameFromSave();
            }
            // Case: save on slot with existing data
            else if (saveSlotUI.HasData)
            {
                SaveGameManager.Instance.ChangeProfileId(saveSlotUI.GetProfileId());
                confirmationPopupUI.gameObject.SetActive(true);
            }
            // Case: save on slot with no data
            else
            {
                SaveGameManager.Instance.ChangeProfileId(saveSlotUI.GetProfileId());
                SaveGameManager.Instance.SaveGame();
            }

            LoadAndDisplaySaveSlotUIs();
        }
        
        // TODO: consider to wait for save complete
        public void ConfirmSave()
        {
            if (!IsConfirmationPopupUIActive) return;
            
            confirmationPopupUI.gameObject.SetActive(false);
            SaveGameManager.Instance.SaveGame();
            LoadAndDisplaySaveSlotUIs();
        }
        
        public void HideConfirmationPopupUI()
        {
            confirmationPopupUI.gameObject.SetActive(false);
        }

        public void Activate(bool isLoadingGameData)
        {
            this.gameObject.SetActive(true);
            this._isLoadingGameData = isLoadingGameData;
            this.saveLoadLabel.text = isLoadingGameData ? "< LOAD" : "< SAVE";
            
            LoadAndDisplaySaveSlotUIs();
        }

        public void Deactivate()
        {
            this.gameObject.SetActive(false);
        }
        
        #endregion
        
        #region Private Methods

        private void LoadAndDisplaySaveSlotUIs()
        {
            Dictionary<string, GameData> profileData = SaveGameManager.Instance.GetAllGameDataProfiles();

            foreach (var saveSlotUI in _saveSlotUIs)
            {
                profileData.TryGetValue(saveSlotUI.GetProfileId(), out var gameData);
                saveSlotUI.SetGameData(gameData);

                bool isInteractable = !_isLoadingGameData || gameData is not null;
                saveSlotUI.SetInteractable(isInteractable);
            }
        }
        
        #endregion
    }
}

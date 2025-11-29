using Settings;
using UnityEngine;

namespace PauseMenu
{
    public class PauseMenuUIController : MonoBehaviour
    {
        [Header("Navigation")]
        [SerializeField] private SaveLoadMenuUIController saveLoadMenuUIController;
        [SerializeField] private SettingsMenu settingsMenu;
        [SerializeField] private GameObject confirmationPopupUI;

        public void OnSaveButtonClicked()
        {
            this.gameObject.SetActive(false);
            saveLoadMenuUIController.Activate(false);
        }
        
        public void OnLoadButtonClicked()
        {
            saveLoadMenuUIController.Activate(true);
            this.gameObject.SetActive(false);
        }

        public void OnSettingsButtonClicked()
        {
            settingsMenu.Activate();
            this.gameObject.SetActive(false);
        }
        
        public void DeactivateSettingsMenu()
        {
            settingsMenu.Deactivate();
            gameObject.SetActive(true);
        }
        
        public void OnQuitToMainMenuButtonClicked()
        {
            this.Deactivate();
            GameManager.Instance.QuitToMainMenu();
        }

        public void ActivateConfirmationPopupUI()
        {
            confirmationPopupUI.gameObject.SetActive(true);
        }
        
        public void DeactivateConfirmationPopupUI()
        {
            confirmationPopupUI.gameObject.SetActive(false);
        }

        public void OnConfirmQuitButtonClicked()
        {
            this.Deactivate();
            GameManager.Instance.QuitToDesktop();
        }

        public void Activate()
        {
            this.gameObject.SetActive(true);
            GameManager.Instance.PauseGame();
        }
        
        public void Deactivate()
        {
            this.gameObject.SetActive(false);
            GameManager.Instance.ResumeGame();
        }
    }
}

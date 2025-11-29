using Characters.PlayerSystem.Input;
using Events;
using Interfaces;
using SaveLoadSystem;
using UnityEngine;

namespace UI
{
    public class DeathScreenUIController : MonoBehaviour, IActivatableUI
    {
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
        
        public void OnContinueButtonClicked()
        {
            Deactivate();
            GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.Player);
            
            SaveGameManager.Instance.ChangeToRecentlyUpdatedProfileId();
            GameManager.Instance.LoadGameFromSave();
        }

        public void OnQuitToMainMenuButtonClicked()
        {
            Deactivate();
            
            GameManager.Instance.QuitToMainMenu();
        }
    }
}

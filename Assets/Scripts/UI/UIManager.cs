using System.Collections;
using Audio;
using Characters.PlayerSystem;
using Characters.PlayerSystem.Input;
using Dialogue.UI;
using Events;
using InventorySystem.Core;
using InventorySystem.UI;
using Items.Core.Interfaces;
using PauseMenu;
using QuestSystem.UI;
using Settings;
using ShopSystem.Core;
using ShopSystem.UI;
using UI.Bar;
using UnityEngine;
using UnityEngine.UI;

// TODO: refactor this
namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
    
        [Header("UI SoundFX")]
        [SerializeField] private UISoundFX uiSoundFX;
        
        [Header("Controllers")]
        [SerializeField] private DeathScreenUIController deathScreenUIController;
        [SerializeField] private InventoryUIController inventoryUIController;
        [SerializeField] private ShopUIController shopUIController;
        [SerializeField] private QuestLogUIController questLogUIController;
        [SerializeField] private PauseMenuUIController pauseMenuUIController;
        [SerializeField] private SaveLoadMenuUIController saveLoadMenuUIController;
        [SerializeField] private SettingsMenu settingsMenu;
        [SerializeField] private DialogueUIController dialogueUIController;
    
        [Header("Damage Overlay")]
        [SerializeField] private Image damageOverlay;
        [SerializeField] private float duration = 2f;
        [SerializeField] private float fadeSpeed = 1f;
        
        [Header("Player Interact Hint")]
        [SerializeField] private InteractHint interactHint;
    
        [Header("Player Stats")]
        [SerializeField] private BarUI staminaBar;
        [SerializeField] private BarUI healthBar;

        private const float DefaultAlpha = 0.8f;
        
        private float _durationTimer;
        private Coroutine _fadeOutCoroutine;
    
        #region Unity Methods
    
        private void Awake()
        {
            if (Instance == null) 
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            damageOverlay.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // Input events
            GameEventManager.Instance.InputEventHandler.HotbarSelected += HandleHotbarSelected;
            GameEventManager.Instance.InputEventHandler.DisplayInventoryUIPerformed += HandleDisplayInventoryUIPerformed;
            GameEventManager.Instance.InputEventHandler.DisplayQuestLogUIPerformed += HandleDisplayQuestLogUIPerformed;
            GameEventManager.Instance.InputEventHandler.DisplayPauseMenuPerformed += HandleDisplayPauseMenuPerformed;
            GameEventManager.Instance.InputEventHandler.CloseActiveUI += HandleCloseActiveUI;
            GameEventManager.Instance.InputEventHandler.ConfirmPerformed += HandleConfirmPerformed;
            
            // Player Events
            GameEventManager.Instance.PlayerEventHandler.CurrentStaminaChanged += OnStaminaChange;
            GameEventManager.Instance.PlayerEventHandler.CurrentHealthChanged += OnHealthChange;
        }

        private void OnDisable()
        {
            // Input events
            GameEventManager.Instance.InputEventHandler.HotbarSelected -= HandleHotbarSelected;
            GameEventManager.Instance.InputEventHandler.DisplayInventoryUIPerformed -= HandleDisplayInventoryUIPerformed;
            GameEventManager.Instance.InputEventHandler.DisplayQuestLogUIPerformed -= HandleDisplayQuestLogUIPerformed;
            GameEventManager.Instance.InputEventHandler.DisplayPauseMenuPerformed -= HandleDisplayPauseMenuPerformed;
            GameEventManager.Instance.InputEventHandler.CloseActiveUI -= HandleCloseActiveUI;
            GameEventManager.Instance.InputEventHandler.ConfirmPerformed -= HandleConfirmPerformed;
            
            // Player Events
            GameEventManager.Instance.PlayerEventHandler.CurrentStaminaChanged -= OnStaminaChange;
            GameEventManager.Instance.PlayerEventHandler.CurrentHealthChanged -= OnHealthChange;
        }
        
        private void Start()
        {
            // Let file handler and containers in settings to load
            settingsMenu.Activate();
            settingsMenu.Deactivate();

            // Let hot bar UI to load
            inventoryUIController.DisplayInventoryUI();
            inventoryUIController.HideInventoryUI();
        }
    
        #endregion

        #region Public Methods

        public void DisplayDamageOverlay()
        {
            if (_fadeOutCoroutine != null)
                StopCoroutine(_fadeOutCoroutine);
            _fadeOutCoroutine = StartCoroutine(FadeOutDamageOverlayRoutine());
        }

        public void DisplayGameOverScreen()
        {
            damageOverlay.gameObject.SetActive(false);
            deathScreenUIController.Activate();
            
            uiSoundFX.PlaySoundFX(AudioManager.Instance.deathSFX);
            
            GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.UI);
        }
        
        public void DisplayInteractHint(IInteractable interactableObject)
        {
            interactHint.DisplayPrompt(interactableObject);
            interactHint.gameObject.SetActive(true);
        }

        public void HideInteractHint()
        {
            interactHint.gameObject.SetActive(false);
        }

        public void DisplayShopUI(ShopContainer shopContainer, PlayerInventoryHolder playerInventoryHolder)
        {
            HandleCloseActiveUI();
            shopUIController.DisplayShopUI(shopContainer, playerInventoryHolder);
            GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.UI);
        }

        public void DisplayChestUI(InventoryContainer chestContainer)
        {
            HandleCloseActiveUI();
            inventoryUIController.DisplayChestUI(chestContainer);
            GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.UI);
        }
    
        #endregion

        #region Private Methods
        
        private void OnStaminaChange(float currentValue, float maxValue)
        {
            var normalizedValue = Mathf.Clamp01(currentValue/maxValue);
            Instance.DisplayCurrentStaminaValue(normalizedValue);
        }

        private void OnHealthChange(float currentValue, float maxValue)
        {
            var normalizedValue = Mathf.Clamp01(currentValue/maxValue);
            Instance.DisplayCurrentHealthValue(normalizedValue);
        }
    
        private void DisplayCurrentStaminaValue(float normalizedValue)
        {
            staminaBar.SetSliderValue(normalizedValue);
        }
    
        private void DisplayCurrentHealthValue(float normalizedValue)
        {
            healthBar.SetSliderValue(normalizedValue);
        }
    
        private void HandleHotbarSelected(int index)
        {
            inventoryUIController.Hotbar.Selected(index);
        }

        private void HandleDisplayInventoryUIPerformed()
        {
            HandleCloseActiveUI();
            inventoryUIController.DisplayInventoryUI();
            GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.UI);
        }

        private void HandleDisplayQuestLogUIPerformed()
        {
            HandleCloseActiveUI();
            questLogUIController.DisplayQuestLogUI();
            GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.UI);
        }
    
        private void HandleDisplayPauseMenuPerformed()
        {
            HandleCloseActiveUI();
            pauseMenuUIController.Activate();
            GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.UI);
        }
    
        // TODO: consider should this handle or let each manager handle its own
        private void HandleCloseActiveUI()
        {
            if (saveLoadMenuUIController.gameObject.activeInHierarchy)
            {
                HandleCloseSaveLoadMenuUI();
            }
            else if (settingsMenu.gameObject.activeInHierarchy)
            {
                settingsMenu.Deactivate();
                pauseMenuUIController.gameObject.SetActive(true);
            }
            else if (dialogueUIController.IsDisplayingDialogueUI)
            {
                dialogueUIController.HideDialogueUI();
            }
            else
            {
                pauseMenuUIController.Deactivate();
                shopUIController.HideShopUI();
                questLogUIController.HideQuestLogUI();
                inventoryUIController.HideInventoryUI();
                inventoryUIController.HideChestUI();
        
                GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.Player);
            }
        }

        private void HandleCloseSaveLoadMenuUI()
        {
            if (saveLoadMenuUIController.IsConfirmationPopupUIActive)
            {
                saveLoadMenuUIController.HideConfirmationPopupUI();
            }
            else
            {
                saveLoadMenuUIController.Deactivate();
                pauseMenuUIController.gameObject.SetActive(true);
            }
        }
    
        private void HandleConfirmPerformed(InputEventContext context)
        { 
            if (!context.Equals(InputEventContext.Default)) return;
            
            shopUIController.ConfirmTrade();
            saveLoadMenuUIController.ConfirmSave();
        }

        private IEnumerator FadeOutDamageOverlayRoutine()
        {
            damageOverlay.gameObject.SetActive(true);

            // Reset image alpha
            Color color = damageOverlay.color;
            color.a = DefaultAlpha;
            damageOverlay.color = color;

            yield return new WaitForSeconds(duration);
            
            while (damageOverlay.color.a > 0)
            {
                float tempAlpha = damageOverlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                damageOverlay.color = new Color(
                    damageOverlay.color.r,
                    damageOverlay.color.g,
                    damageOverlay.color.b,
                    tempAlpha
                );

                yield return null;
            }
            
            damageOverlay.gameObject.SetActive(false);
        }
    
        #endregion
    }
}

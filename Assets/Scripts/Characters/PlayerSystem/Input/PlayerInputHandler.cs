using Characters.PlayerSystem.Animators;
using Characters.PlayerSystem.Input.Managers;
using Events;
using PlayerSystem.Input.Managers;
using UnityEngine;

namespace Characters.PlayerSystem.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private bool _isActionMapInitialized = false;
        
        // Input actions - Unity auto generated script
        private PlayerInputActions _inputActions;
        
        // Input managers
        private MovementInputManager _movementInputManager;
        private CameraInputManager _cameraInputManager;
        private InteractionInputManager _interactionInputManager;
        private CombatInputManager _combatInputManager;
        private UIInputManager _uiInputManager;
        private GeneralInputManager _generalInputManager;
        
        // Player components
        private PlayerCharacter _playerCharacter;
        private PlayerCamera _playerCamera;
        private PlayerInteract _playerInteract;
        private ArmsPlayerAnimator _armsAnimator;
        private ShadowPlayerAnimator _shadowAnimator;

        // Input map state
        private ActionMap _currentActionMap = ActionMap.Player;
        
        #region Unity Methods
        
        private void Awake()
        {
            InitializeInputActions();
            InitializeInputManagers();
        }

        // TODO: consider to get component
        public void Initialize(PlayerCharacter playerCharacter, PlayerCamera playerCamera, PlayerInteract playerInteract
            , ArmsPlayerAnimator armsAnimator, ShadowPlayerAnimator shadowAnimator)
        {
            _playerCharacter = playerCharacter;
            _playerCamera = playerCamera;
            _playerInteract = playerInteract;
            
            _armsAnimator = armsAnimator;
            _shadowAnimator = shadowAnimator;
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            
            _generalInputManager?.Enable(); // Always enable general input manager for any input action map
            SetActionMap(_currentActionMap);
            
            GameEventManager.Instance.UIEventHandler.ActionMapChangeRequested += SetActionMap;
        }

        private void OnDisable()
        {
            _movementInputManager?.Disable();
            _combatInputManager?.Disable();
            _uiInputManager?.Disable();
            _generalInputManager?.Disable();
        
            _inputActions.Disable();
            
            GameEventManager.Instance.UIEventHandler.ActionMapChangeRequested -= SetActionMap;
        }
        
        #endregion

        #region Public Methods
        
        public void UpdateInputs()
        {
            if (GameManager.Instance.IsGamePaused || _currentActionMap != ActionMap.Player) return;
            
            // Update Input Data
            _cameraInputManager.UpdateCameraInput();
            _movementInputManager.UpdateMovementInput(_playerCamera.transform.rotation);
            _interactionInputManager.UpdateInteractionInput();
            
            // Update Player Components with related Input Data
            UpdatePlayerComponents();
        }
        
        #endregion
        
        #region Private Methods
        
        private void InitializeInputActions()
        {
            if (_inputActions == null)
            {
                _inputActions = new PlayerInputActions();    
            }
        }
    
        private void InitializeInputManagers()
        {
            _movementInputManager = new MovementInputManager(_inputActions);
            _cameraInputManager = new CameraInputManager(_inputActions);
            _interactionInputManager = new InteractionInputManager(_inputActions);
            _combatInputManager = new CombatInputManager(_inputActions);
            _uiInputManager = new UIInputManager(_inputActions);
            _generalInputManager = new GeneralInputManager(_inputActions);
        }

        private void UpdatePlayerComponents()
        {
            _playerCharacter.UpdateMovement(_movementInputManager.MovementInputData, Time.deltaTime);
            _playerCamera.UpdateCamera(_cameraInputManager.CameraInputData);
            
            _armsAnimator.UpdateAnimation(_movementInputManager.MovementInputData);
            _shadowAnimator.UpdateAnimation(_movementInputManager.MovementInputData);
            
            _playerInteract.UpdateInteract(_interactionInputManager.InteractionInputData);
        }
        
        private void SetActionMap(ActionMap newActionMap)
        {
            if (_isActionMapInitialized && _currentActionMap == newActionMap) return;
            
            DisableCurrentInputActionMap();
            
            _currentActionMap = newActionMap;
            _isActionMapInitialized = true;
            switch (_currentActionMap)
            {
                case ActionMap.Player:
                    _inputActions.Player.Enable();
                    _inputActions.UI.Disable();
                    
                    EnableInputManagersForPlayerMap();
                    SetCursorForPlayerMap();
                    break;
                case ActionMap.UI:
                    _playerCharacter.ClearRequestedInputs();
                    
                    _inputActions.Player.Disable();
                    _inputActions.UI.Enable();
                    
                    EnableInputManagersForUIMap();
                    SetCursorForUIMap();
                    break;
            }
        }

        // Player map related methods
        private void EnableInputManagersForPlayerMap()
        {
            _movementInputManager?.Enable();
            _combatInputManager?.Enable();
        }

        private void DisableInputManagersForPlayerMap()
        {
            _movementInputManager?.Disable();
            _combatInputManager?.Disable();
        }
        
        private void SetCursorForPlayerMap()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // UI map related methods
        private void EnableInputManagersForUIMap()
        {
            _uiInputManager?.Enable();
        }

        private void DisableInputManagersForUIMap()
        {
            _uiInputManager?.Disable();
        }

        private void SetCursorForUIMap()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        private void DisableCurrentInputActionMap()
        {
            switch (_currentActionMap)
            {
                case ActionMap.Player:
                    DisableInputManagersForPlayerMap();
                    break;
                case ActionMap.UI:
                    DisableInputManagersForUIMap();
                    break;
            }
        }
        
        #endregion
    }
}

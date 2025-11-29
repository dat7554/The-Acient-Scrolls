using Characters.PlayerSystem.Input.Data;
using Events;
using PlayerSystem.Input.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.PlayerSystem.Input.Managers
{
    /// <summary>
    /// Manages movement-related input - handles discrete events and update continuous events
    /// </summary>
    public class MovementInputManager
    {
        private readonly PlayerInputActions _inputActions;

        public MovementInput MovementInputData { get; private set; }

        public MovementInputManager(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
        }
    
        public void Enable()
        {
            _inputActions.Player.Jump.started += OnJumpStarted;
            _inputActions.Player.Jump.canceled += OnJumpCanceled;
        
            _inputActions.Player.Crouch.started += OnCrouchStarted;
            _inputActions.Player.Crouch.canceled += OnCrouchCanceled;
        
            _inputActions.Player.Sprint.started += OnSprintStarted;
            _inputActions.Player.Sprint.canceled += OnSprintCanceled;
        }

        public void Disable()
        {
            _inputActions.Player.Jump.started -= OnJumpStarted;
            _inputActions.Player.Jump.canceled -= OnJumpCanceled;
        
            _inputActions.Player.Crouch.started -= OnCrouchStarted;
            _inputActions.Player.Crouch.canceled -= OnCrouchCanceled;
        
            _inputActions.Player.Sprint.started -= OnSprintStarted;
            _inputActions.Player.Sprint.canceled -= OnSprintCanceled;
        }
        
        public void UpdateMovementInput(Quaternion rotation)
        {
            var horizontalMovement = _inputActions.Player.Move.ReadValue<Vector2>();
            MovementInputData = new MovementInput (rotation, horizontalMovement);
        }

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeJumpStarted();
        }
    
        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeJumpCanceled();
        }

        private void OnCrouchStarted(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeCrouchStarted();
        }
    
        private void OnCrouchCanceled(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeCrouchCanceled();
        }

        private void OnSprintStarted(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeSprintStarted();
        }
    
        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeSprintCanceled();
        }
    }
}
using System;
using Events;
using UnityEngine.InputSystem;

namespace PlayerSystem.Input.Managers
{
    public class UIInputManager
    {
        private readonly PlayerInputActions _inputActions;

        public UIInputManager(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
        }

        public void Enable()
        {
            // Close & Confirm
            _inputActions.UI.Close.performed += OnCloseActiveUI;
            _inputActions.UI.Confirm.performed += OnConfirmPerformed;
            
            // Drop Item
            _inputActions.UI.DropItem.performed += OnDropItemPerformed;
        }

        public void Disable()
        {
            // Close & Confirm
            _inputActions.UI.Close.performed -= OnCloseActiveUI;
            _inputActions.UI.Confirm.performed -= OnConfirmPerformed;
            
            // Drop Item
            _inputActions.UI.DropItem.performed -= OnDropItemPerformed;
        }

        private void OnCloseActiveUI(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeCloseActiveUI();
        }

        private void OnConfirmPerformed(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeConfirmPerformed();
        }

        private void OnDropItemPerformed(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeDropItemPerformed();
        }
    }
}
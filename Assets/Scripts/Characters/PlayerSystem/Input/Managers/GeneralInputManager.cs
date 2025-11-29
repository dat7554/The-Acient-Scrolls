using System;
using Events;
using UnityEngine.InputSystem;

namespace PlayerSystem.Input.Managers
{
    public class GeneralInputManager
    {
        private readonly PlayerInputActions _inputActions;

        private readonly Action<InputAction.CallbackContext>[] _hotbarHandlers;
        
        public GeneralInputManager(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
            
            _hotbarHandlers = new Action<InputAction.CallbackContext>[6];
            for (int i = 0; i < _hotbarHandlers.Length; i++)
            {
                var index = i;
                _hotbarHandlers[i] = context => OnHotbarSelected(index);
            }
        }

        public void Enable()
        {
            // Hotbar
            _inputActions.Player.Hotbar1.performed += _hotbarHandlers[0];
            _inputActions.Player.Hotbar2.performed += _hotbarHandlers[1];
            _inputActions.Player.Hotbar3.performed += _hotbarHandlers[2];
            _inputActions.Player.Hotbar4.performed += _hotbarHandlers[3];
            _inputActions.Player.Hotbar5.performed += _hotbarHandlers[4];
            _inputActions.Player.Hotbar6.performed += _hotbarHandlers[5];
            
            // Inventory UI
            _inputActions.Player.Inventory.performed += OnDisplayInventoryUIPerformed;
            
            // Quest Log UI
            _inputActions.Player.ToggleQuestLog.performed += OnDisplayQuestLogUIPerformed;
            
            // Pause Menu
            _inputActions.Player.PauseMenu.performed += OnDisplayPauseMenuPerformed;
        }

        public void Disable()
        {
            // Hotbar
            _inputActions.Player.Hotbar1.performed -= _hotbarHandlers[0];
            _inputActions.Player.Hotbar2.performed -= _hotbarHandlers[1];
            _inputActions.Player.Hotbar3.performed -= _hotbarHandlers[2];
            _inputActions.Player.Hotbar4.performed -= _hotbarHandlers[3];
            _inputActions.Player.Hotbar5.performed -= _hotbarHandlers[4];
            _inputActions.Player.Hotbar6.performed -= _hotbarHandlers[5];
            
            // Inventory UI
            _inputActions.Player.Inventory.performed -= OnDisplayInventoryUIPerformed;
            
            // Quest Log UI
            _inputActions.Player.ToggleQuestLog.performed -= OnDisplayQuestLogUIPerformed;
            
            // Pause Menu
            _inputActions.Player.PauseMenu.performed -= OnDisplayPauseMenuPerformed;
        }
        
        private void OnHotbarSelected(int index)
        {
            GameEventManager.Instance.InputEventHandler.InvokeHotbarSelected(index);
        }

        private void OnDisplayInventoryUIPerformed(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeDisplayInventoryUIPerformed();
        }
        
        private void OnDisplayQuestLogUIPerformed(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeDisplayQuestLogUIPerformed();
        }
        
        private void OnDisplayPauseMenuPerformed(InputAction.CallbackContext context)
        {
            GameEventManager.Instance.InputEventHandler.InvokeDisplayPauseMenuPerformed();
        }
    }
}
using Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Characters.PlayerSystem.Input.Managers
{
    public class CombatInputManager
    {
        private readonly PlayerInputActions _inputActions;

        public CombatInputManager(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
        }

        public void Enable()
        {
            _inputActions.Player.DrawWeapon.performed += OnToggleWeaponPerformed;
            _inputActions.Player.Attack.performed += OnAttackPerformed;
            _inputActions.Player.Block.started += OnBlockStarted;
            _inputActions.Player.Block.canceled += OnBlockCanceled;
        }

        public void Disable()
        {
            _inputActions.Player.DrawWeapon.performed -= OnToggleWeaponPerformed;
            _inputActions.Player.Attack.performed -= OnAttackPerformed;
            _inputActions.Player.Block.started -= OnBlockStarted;
            _inputActions.Player.Block.canceled -= OnBlockCanceled;
        }

        private void OnToggleWeaponPerformed(InputAction.CallbackContext context) 
            => GameEventManager.Instance.InputEventHandler.InvokeToggleWeaponPerformed();

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            switch (context.interaction)
            {
                case TapInteraction:
                    GameEventManager.Instance.InputEventHandler.InvokeAttackPerformed(AttackType.Light);
                    break;
                case PressInteraction:
                    GameEventManager.Instance.InputEventHandler.InvokeAttackPerformed(AttackType.Heavy);
                    break;
                default:
                    GameEventManager.Instance.InputEventHandler.InvokeAttackPerformed(AttackType.Light);
                    break;
            }
        }

        private void OnBlockStarted(InputAction.CallbackContext context) 
            => GameEventManager.Instance.InputEventHandler.InvokeBlockStarted();

        private void OnBlockCanceled(InputAction.CallbackContext context) 
            => GameEventManager.Instance.InputEventHandler.InvokeBlockCanceled();
    }
}
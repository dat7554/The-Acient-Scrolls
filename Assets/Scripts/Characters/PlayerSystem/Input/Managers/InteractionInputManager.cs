using PlayerSystem.Input.Data;

namespace PlayerSystem.Input.Managers
{
    public class InteractionInputManager
    {
        private readonly PlayerInputActions _inputActions;
        
        public InteractionInput InteractionInputData { get; private set; }
        
        public InteractionInputManager(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
        }
        
        public void UpdateInteractionInput()
        {
            bool interactPressed = _inputActions.Player.Interact.WasPressedThisFrame();
            bool grabHeld = _inputActions.Player.Grab.IsPressed();

            InteractionInputData = new InteractionInput(interactPressed, grabHeld);
        }
    }
}
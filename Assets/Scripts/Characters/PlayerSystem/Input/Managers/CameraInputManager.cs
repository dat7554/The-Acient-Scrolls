using PlayerSystem.Input.Data;
using UnityEngine;

namespace PlayerSystem.Input.Managers
{
    public class CameraInputManager
    {
        private readonly PlayerInputActions _inputActions;
        
        public CameraInput CameraInputData { get; private set; }

        public CameraInputManager(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
        }
        
        public void UpdateCameraInput()
        {
            var cameraLook = _inputActions.Player.Look.ReadValue<Vector2>();
            CameraInputData = new CameraInput(cameraLook);
        }
    }
}
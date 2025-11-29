using System;
using UnityEngine;

namespace PlayerSystem.Input.Data
{
    [Serializable]
    public struct CameraInput
    {
        public Vector2 cameraLook;

        public CameraInput(Vector2 cameraLook)
        {
            this.cameraLook = cameraLook;
        }
    }
}
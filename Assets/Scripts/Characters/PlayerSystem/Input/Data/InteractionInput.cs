using System;

namespace PlayerSystem.Input.Data
{
    [Serializable]
    public struct InteractionInput
    {
        public bool interactPressed;
        public bool grabHeld;

        public InteractionInput(bool interactPressed, bool grabHeld)
        {
            this.interactPressed = interactPressed;
            this.grabHeld = grabHeld;
        }
    }
}
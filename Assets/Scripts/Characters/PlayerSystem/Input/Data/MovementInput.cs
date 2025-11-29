using System;
using UnityEngine;

namespace Characters.PlayerSystem.Input.Data
{
    [Serializable]
    public struct MovementInput
    {
        public Quaternion characterRotation;
        public Vector2 horizontalMovement;

        public MovementInput(Quaternion characterRotation, Vector2 horizontalMovement)
        {
            this.characterRotation = characterRotation;
            this.horizontalMovement = horizontalMovement;
        }
    }
}
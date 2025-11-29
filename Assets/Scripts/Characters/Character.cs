using UnityEngine;

namespace Characters
{
    public abstract class Character : MonoBehaviour
    {
        public virtual Vector3 ForwardTransform => transform.forward;
        
        public abstract FactionType Faction { get; }
    
        /// <summary>
        /// Called when this character successfully blocks an incoming melee attack.
        /// </summary>
        public abstract void HandleSuccessfulBlock();
        
        /// <summary>
        /// Called when this character’s melee attack hit Player layer, NPC layer, or Shield Collider layer.
        /// </summary>
        public abstract void HandleAttackHit();
    }
}

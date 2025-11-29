using Interfaces;
using UnityEngine;

namespace Characters.NPC.Enemy
{
    public abstract class EnemyNPCAnimatorEventReceiver : MonoBehaviour, ICombatAnimatorEvents
    {
        /// <summary>
        /// Starts damage detection for the specified weapon hit type in an attack animation event.
        /// </summary>
        /// <param name="hitType">The specified weapon hit type</param>
        public virtual void OnDamageStart(int hitType) { /* no operation */ }

        /// <summary>
        /// Ends damage detection for the specified weapon hit type in an attack animation event.
        /// </summary>
        /// <param name="hitType">The specified weapon hit type</param>
        public virtual void OnDamageEnd(int hitType) { /* no operation */ }
    }
}

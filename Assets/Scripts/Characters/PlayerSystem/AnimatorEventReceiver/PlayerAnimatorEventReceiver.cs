using Interfaces;
using UnityEngine;

namespace Characters.PlayerSystem.AnimatorEventReceiver
{
    public abstract class PlayerAnimatorEventReceiver : MonoBehaviour, ICombatAnimatorEvents
    {
        protected PlayerEquipment Equipment;
        protected Player player;
        
        public void Initialize(Player _player, PlayerEquipment playerEquipment)
        {
            this.player = _player;
            Equipment = playerEquipment;
        }

        public abstract void PlayLandSoundFX();
        
        public abstract void OnDrawWeaponEvent();
        public abstract void OnSheathWeaponEvent();

        public abstract void OnDrawShieldEvent();
        public abstract void OnSheathShieldEvent();
        
        public abstract void OnDamageStart(int hitType);
        public abstract void OnDamageEnd(int hitType);
        
        public abstract void OnBlockStart();
        public abstract void OnBlockEnd();
    }
}

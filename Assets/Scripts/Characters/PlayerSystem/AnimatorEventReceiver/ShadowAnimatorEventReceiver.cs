namespace Characters.PlayerSystem.AnimatorEventReceiver
{
    public class ShadowAnimatorEventReceiver : PlayerAnimatorEventReceiver
    {
        public override void PlayLandSoundFX() { /* no operation */ }
        
        public override void OnDrawWeaponEvent() => Equipment.DrawWeaponOnShadow();
        public override void OnSheathWeaponEvent() => Equipment.SheathWeaponOnShadow();
        
        public override void OnDrawShieldEvent() => Equipment.DrawShieldOnShadow();
        public override void OnSheathShieldEvent() => Equipment.SheathShieldOnShadow();

        public override void OnDamageStart(int hitType) { /* no operation */ }
        public override void OnDamageEnd(int hitType) { /* no operation */ }
        
        public override void OnBlockStart() { /* no operation */ }
        public override void OnBlockEnd() { /* no operation */ }
    }
}
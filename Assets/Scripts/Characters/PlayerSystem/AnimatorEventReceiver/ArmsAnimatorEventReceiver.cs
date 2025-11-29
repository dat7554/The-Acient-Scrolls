namespace Characters.PlayerSystem.AnimatorEventReceiver
{
    public class ArmsAnimatorEventReceiver : PlayerAnimatorEventReceiver
    {
        public override void PlayLandSoundFX() => player.PlayLandSoundFX();
        
        public override void OnDrawWeaponEvent() => Equipment.DrawWeaponOnArms();
        public override void OnSheathWeaponEvent() => Equipment.SheathWeaponOnArms();

        public override void OnDrawShieldEvent() => Equipment.DrawShieldOnArms();
        public override void OnSheathShieldEvent() => Equipment.SheathShieldOnArms();

        public override void OnDamageStart(int hitType)
        {
            player.PlayWeaponWhooshSoundFX();
            Equipment.CurrentRightHandMeleeWeaponComponent.StartBladeDamage();
        }
        public override void OnDamageEnd(int hitType) => Equipment.CurrentRightHandMeleeWeaponComponent.EndBladeDamage();

        public override void OnBlockStart()
        {
            /* no operation */
            // logic is implemented in ArmsPlayerAnimator as currently no suitable shield draw/sheath animations
        }
        public override void OnBlockEnd()
        {
            /* no operation */
            // logic is implemented in ArmsPlayerAnimator as currently no suitable shield draw/sheath animations
        }
    }
}
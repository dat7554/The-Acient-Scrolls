namespace Interfaces
{
    public interface ICombatAnimatorEvents
    {
        void OnDamageStart(int hitType);
        void OnDamageEnd(int hitType);
    }
}

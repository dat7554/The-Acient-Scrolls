namespace Characters.NPC.Enemy
{
    public class EnemyArcherAnimatorEventReceiver : EnemyNPCAnimatorEventReceiver
    {
        private EnemyArcher _enemyArcher;

        private void Awake()
        {
            _enemyArcher = GetComponent<EnemyArcher>();
        }

        public override void OnDamageStart(int hitType)
        {
            _enemyArcher.LeftRangeWeapon.Shoot();
        }
    }
}

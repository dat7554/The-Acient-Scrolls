using Characters;
using UnityEngine;

namespace Items.Components.Weapons
{
    public class RangeWeapon : Weapon
    {
        [SerializeField] private ArrowPool arrowPool;
        
        [Header("Debug")]
        [SerializeField] private Transform arrowPlacementPosition;

        protected void Start()
        {
            Owner = GetComponentInParent<Character>();
        }

        public void Shoot()
        {
            if (arrowPool.Pool == null) return;
            
            Arrow arrow = arrowPool.Pool.Get();
            
            if (!arrow) 
                return;
            
            arrow.transform.SetPositionAndRotation(arrowPlacementPosition.position, arrowPlacementPosition.rotation);
            arrow.Launch(arrowPlacementPosition, Owner);
        }
    }
}

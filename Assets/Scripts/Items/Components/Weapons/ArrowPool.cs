using UnityEngine;
using UnityEngine.Pool;

namespace Items.Components.Weapons
{
    public class ArrowPool : MonoBehaviour
    {
        [SerializeField] private Arrow arrowPrefab;
        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxSize = 100;

        public IObjectPool<Arrow> Pool { get; private set; }

        private void Awake()
        {
            Pool = new ObjectPool<Arrow>
                (
                    CreateArrow,
                    OnGetFromPool,
                    OnReleaseToPool,
                    OnDestroyArrow,
                    collectionCheck: true,
                    defaultCapacity,
                    maxSize
                );
        }

        // Invoked when creating an item to populate the object pool
        private Arrow CreateArrow()
        {
            Arrow arrow = Instantiate(arrowPrefab);
            arrow.Pool = Pool;
            return arrow;
        }
        
        // Invoked when retrieving the next item from the object pool
        private void OnGetFromPool(Arrow arrow)
        {
            arrow.gameObject.SetActive(true);
        }
        
        // Invoked when returning an item to the object pool
        private void OnReleaseToPool(Arrow arrow)
        {
            arrow.gameObject.SetActive(false);
        }

        // Invoked when we exceed the maximum number of pooled items
        private void OnDestroyArrow(Arrow arrow)
        {
            Destroy(arrow.gameObject);
        }
    }
}

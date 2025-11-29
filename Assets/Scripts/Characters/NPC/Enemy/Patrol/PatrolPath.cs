using System.Collections.Generic;
using UnityEngine;

namespace Characters.NPC.Enemy.Patrol
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] private List<Vector3> patrolPoints = new ();

        public List<Vector3> PatrolPoints => patrolPoints;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                patrolPoints.Add(transform.GetChild(i).position);
            }
        }
    }
}

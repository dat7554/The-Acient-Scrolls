using UnityEditor;
using UnityEngine;

namespace QuestSystem.Core
{
    [CreateAssetMenu(fileName = "QuestInfoSO", menuName = "QuestSystem/QuestInfoSO", order = 0)]
    public class QuestInfoSO : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(5,10)] private string description;
        
        [Header("Requirements")]
        [SerializeField] private QuestInfoSO[] questPrerequisites;
        
        [Header("Steps")]
        [SerializeField] private GameObject[] questSteps;
        
        [Header("Rewards")]
        [SerializeField] private int goldReward;
        
        public string ID => id;
        public string DisplayName => displayName;
        public string Description => description;
        public QuestInfoSO[] QuestPrerequisites => questPrerequisites;
        public GameObject[] QuestSteps => questSteps;
        public int GoldReward => goldReward;

        private void OnValidate()
        {
            #if UNITY_EDITOR
            id = this.name;
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}
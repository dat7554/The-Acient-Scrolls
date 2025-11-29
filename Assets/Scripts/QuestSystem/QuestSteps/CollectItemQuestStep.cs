using Items.ScriptableObjects;
using QuestSystem.Core;
using UnityEngine;

namespace QuestSystem.QuestSteps
{
    public abstract class CollectItemQuestStep : QuestStep
    {
        [SerializeField] protected ItemSO itemToCollect;
        [SerializeField] protected int amountRequired;
        
        public ItemSO ItemToCollect => itemToCollect;
        public int AmountRequired => amountRequired;
    }
}

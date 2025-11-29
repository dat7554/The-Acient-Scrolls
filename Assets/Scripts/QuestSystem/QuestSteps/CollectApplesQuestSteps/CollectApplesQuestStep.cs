using System;
using Events;
using Items.ScriptableObjects;

namespace QuestSystem.QuestSteps.CollectApplesQuestSteps
{
    public class CollectApplesQuestStep : CollectItemQuestStep
    {
        private int _applesCollected = 0;

        private void Awake()
        {
            if (amountRequired == 0)
                amountRequired = 5;
        }

        private void OnEnable()
        {
            GameEventManager.Instance.MiscEventHandler.ItemCollected += OnItemCollected;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.MiscEventHandler.ItemCollected -= OnItemCollected;
        }
        
        private void Start()
        {
            UpdateState();
        }
        
        private void OnItemCollected(ItemSO itemData)
        {
            if (itemData.ItemID != itemToCollect.ItemID) return;
            
            if (_applesCollected < amountRequired)
            {
                _applesCollected++;
                UpdateState();
            }

            if (_applesCollected >= amountRequired)
            {
                FinishQuestStep();
            }
        }

        private void UpdateState()
        {
            var state = _applesCollected.ToString();
            var status = "Collected " + _applesCollected + " / " + amountRequired + " apples.";
            ChangeState(state, status);
        }

        protected override void SetQuestStepState(string newState)
        {
            _applesCollected = int.Parse(newState);
            UpdateState();
        }
    }
}

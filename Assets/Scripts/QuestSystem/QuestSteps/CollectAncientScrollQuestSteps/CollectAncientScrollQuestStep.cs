using Events;
using Items.ScriptableObjects;

namespace QuestSystem.QuestSteps.CollectAncientScrollQuestSteps
{
    public class CollectAncientScrollQuestStep : CollectItemQuestStep
    {
        private int _scrollCollected = 0;
        
        private void Awake()
        {
            if (amountRequired == 0)
                amountRequired = 1;
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
            
            if (_scrollCollected < amountRequired)
            {
                _scrollCollected++;
                UpdateState();
            }

            if (_scrollCollected >= amountRequired)
            {
                FinishQuestStep();
            }
        }

        private void UpdateState()
        {
            var state = _scrollCollected.ToString();
            var status = "Collected " + _scrollCollected + " / " + amountRequired + " scroll.";
            ChangeState(state, status);
        }

        protected override void SetQuestStepState(string newState)
        {
            _scrollCollected = int.Parse(newState);
            UpdateState();
        }
    }
}

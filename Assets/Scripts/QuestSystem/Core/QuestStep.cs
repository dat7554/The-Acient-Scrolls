using Events;
using UnityEngine;

namespace QuestSystem.Core
{
    public abstract class QuestStep : MonoBehaviour
    {
        private bool _isFinished;
        private string _questID;
        private int _stepIndex;

        public void Initialize(string id, int stepIndex, string questStepState)
        {
            this._questID = id;
            this._stepIndex = stepIndex;

            if (questStepState is not null && questStepState != "")
            {
                SetQuestStepState(questStepState);
            }
        }
        
        protected void FinishQuestStep()
        {
            if (_isFinished) return;
            
            _isFinished = true;
            GameEventManager.Instance.QuestEventHandler.InvokeQuestAdvanced(_questID);
            Destroy(this.gameObject);
        }

        protected void ChangeState(string newState, string newStatus)
        {
            GameEventManager.Instance.QuestEventHandler.InvokeQuestStepStateChanged
                (
                    _questID, 
                    _stepIndex, 
                    new QuestStepState(newState, newStatus)
                );
        }

        protected abstract void SetQuestStepState(string newState);
    }
}

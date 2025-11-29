using System;
using QuestSystem.Core;

namespace Events
{
    public class QuestEvent
    {
        #region Events
        
        public event Action<string> QuestStarted;
        public event Action<string> QuestAdvanced;
        public event Action<string> QuestFinished;
        
        public event Action<Quest> QuestStateChanged;
        public event Action<string, int, QuestStepState> QuestStepStateChanged;
        
        #endregion
        
        #region Event invocation methods
        
        public void InvokeQuestStarted(string id) => QuestStarted?.Invoke(id);
        public void InvokeQuestAdvanced(string id) => QuestAdvanced?.Invoke(id);
        public void InvokeQuestFinished(string id) => QuestFinished?.Invoke(id);

        public void InvokeQuestStateChanged(Quest quest) => QuestStateChanged?.Invoke(quest);
        public void InvokeQuestStepStateChanged(string questID, int stepIndex, QuestStepState questStepState) 
            => QuestStepStateChanged?.Invoke(questID, stepIndex, questStepState);

        #endregion
    }
}
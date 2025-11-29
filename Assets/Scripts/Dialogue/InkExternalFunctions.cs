using Events;
using Ink.Runtime;

namespace Dialogue
{
    public class InkExternalFunctions
    {
        public void Bind(Story story)
        {
            story.BindExternalFunction("StartQuest", (string questID) => StartQuest(questID));
            story.BindExternalFunction("AdvanceQuest", (string questID) => AdvanceQuest(questID));
            story.BindExternalFunction("FinishQuest", (string questID) => FinishQuest(questID));
        }

        public void Unbind(Story story)
        {
            story.UnbindExternalFunction("StartQuest");
            story.UnbindExternalFunction("AdvanceQuest");
            story.UnbindExternalFunction("FinishQuest");
        }
        
        private void StartQuest(string questID)
        {
            GameEventManager.Instance.QuestEventHandler.InvokeQuestStarted(questID);
        }

        private void AdvanceQuest(string questID)
        {
            GameEventManager.Instance.QuestEventHandler.InvokeQuestAdvanced(questID);
        }

        private void FinishQuest(string questID)
        {
            GameEventManager.Instance.QuestEventHandler.InvokeQuestFinished(questID);
        }
    }
}

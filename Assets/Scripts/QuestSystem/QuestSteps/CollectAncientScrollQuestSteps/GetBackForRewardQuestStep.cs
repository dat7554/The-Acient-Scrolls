using QuestSystem.Core;
using UnityEngine;

namespace QuestSystem.QuestSteps.CollectAncientScrollSteps
{
    public class GetBackForRewardQuestStep : QuestStep
    {
        [SerializeField] private string status = "Get back for reward";

        private void Start()
        {
            ChangeState("", status);
            FinishQuestStep();
        }

        protected override void SetQuestStepState(string newState) { /* no-op */ }
    }
}

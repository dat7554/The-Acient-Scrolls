using System;
using QuestSystem.Core;

namespace QuestSystem.DataPersistence
{
    [Serializable]
    public struct QuestData
    {
        public QuestState questState;
        public int questStepIndex;
        public QuestStepState[] questStepStatesArray;

        public QuestData(QuestState questState, int questStepIndex, QuestStepState[] questStepStatesArray)
        {
            this.questState = questState;
            this.questStepIndex = questStepIndex;
            this.questStepStatesArray = questStepStatesArray;
        }
    }
}

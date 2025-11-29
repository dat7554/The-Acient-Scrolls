using QuestSystem.DataPersistence;
using UnityEngine;

namespace QuestSystem.Core
{
    public class Quest
    {
        // Static info of the quest
        public QuestInfoSO QuestInfoData;
        
        // State info of the quest
        public QuestState State;
        private int _currentQuestStepIndex;
        private QuestStepState[] _questStepStatesArray;

        public int CurrentQuestStepIndex => _currentQuestStepIndex;
        public QuestStepState[] QuestStepStatesArray => _questStepStatesArray;
        
        public QuestDataHandler QuestDataHandler { get; private set; }

        public Quest(QuestInfoSO questInfoData)
        {
            this.QuestInfoData = questInfoData;
            this.State = QuestState.RequirementsNotMet;
            this._currentQuestStepIndex = 0;
            this._questStepStatesArray = new QuestStepState[questInfoData.QuestSteps.Length];
            for (var i = 0; i < _questStepStatesArray.Length; i++)
            {
                _questStepStatesArray[i] = new QuestStepState();
            }

            QuestDataHandler = new QuestDataHandler(this);
        }

        public void MoveToNextQuestStep()
        {
            _currentQuestStepIndex++;
        }

        public bool CurrentQuestStepExists()
        {
            return _currentQuestStepIndex < QuestInfoData.QuestSteps.Length;
        }
        
        public void InstantiateCurrentQuestStep(Transform parentTransform)
        {
            var questStepPrefab = GetCurrentQuestStepPrefab();

            if (questStepPrefab is null) return;
            var questStep = Object.Instantiate(questStepPrefab, parentTransform).GetComponent<QuestStep>();

            questStep.Initialize(QuestInfoData.ID, _currentQuestStepIndex, _questStepStatesArray[_currentQuestStepIndex].state);
        }

        public void StoreQuestStepState(QuestStepState questStepState, int questStepIndex)
        {
            if (questStepIndex < QuestInfoData.QuestSteps.Length)
            {
                _questStepStatesArray[questStepIndex].state = questStepState.state;
                _questStepStatesArray[questStepIndex].status = questStepState.status;
            }
            else
            {
                Debug.LogWarning($"Quest: questStepIndex {questStepIndex} out of range in {QuestInfoData.ID}");
            }
        }

        public void LoadQuestData(QuestData questData)
        {
            this.State = questData.questState;
            this._currentQuestStepIndex = questData.questStepIndex;
            this._questStepStatesArray = questData.questStepStatesArray;

            if (_questStepStatesArray.Length != QuestInfoData.QuestSteps.Length)
            {
                Debug.LogWarning($"Quest Step States and Quest Step Prefabs are not same quantity. " +
                                 $"This indicate that QuestInfoSO has been changed and saved data is out of sync." +
                                 $"Reset data at {QuestInfoData.ID} !");
            }
        }

        public string GetAllStepStateStatus()
        {
            var status = "";

            if (State is QuestState.InProgress or QuestState.CanFinish or QuestState.Finished)
            {
                int lastIndex = -1;
                
                if (State is QuestState.CanFinish)
                {
                    lastIndex = _questStepStatesArray.Length - 1;
                }
                
                for (var i = 0; i < _currentQuestStepIndex; i++)
                {
                    if (lastIndex != -1 && i == lastIndex)
                    {
                        status += _questStepStatesArray[i].status + "\n";
                    }
                    else
                    {
                        status += "<s>" + _questStepStatesArray[i].status + "</s>\n";
                    }
                }

                if (CurrentQuestStepExists())
                {
                    status += _questStepStatesArray[_currentQuestStepIndex].status;
                }
            }
            
            return status;
        }
        
        private GameObject GetCurrentQuestStepPrefab()
        {
            GameObject currentQuestStepPrefab = null;
            
            // Step back to let last step calls Start()
            if (State == QuestState.CanFinish)
            {
                _currentQuestStepIndex--;
                State = QuestState.InProgress; // to be instantiated by CreateSlotUI() in QuestLogList
            }
            
            if (CurrentQuestStepExists())
            {
                currentQuestStepPrefab = QuestInfoData.QuestSteps[_currentQuestStepIndex];
            }
            else
            {
                Debug.LogWarning($"No quest step found on {QuestInfoData.ID} at index {_currentQuestStepIndex}");
            }
            
            return currentQuestStepPrefab;
        }
    }
}

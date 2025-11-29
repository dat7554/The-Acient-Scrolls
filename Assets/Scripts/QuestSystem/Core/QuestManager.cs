using System.Collections.Generic;
using Events;
using QuestSystem.QuestSteps;
using SaveLoadSystem;
using UnityEngine;

namespace QuestSystem.Core
{
    // TODO: move save quest to a function, current will act as auto save
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private string questInfoDataFolderPath = "Quests";
        
        private Dictionary<string, Quest> _questDictionary = new Dictionary<string, Quest>();

        private void Awake()
        {
            _questDictionary = CreateQuestDictionary();
        }

        private void OnEnable()
        {
            GameEventManager.Instance.QuestEventHandler.QuestStarted += OnQuestStarted;
            GameEventManager.Instance.QuestEventHandler.QuestAdvanced += OnQuestAdvanced;
            GameEventManager.Instance.QuestEventHandler.QuestFinished += OnQuestFinished;

            GameEventManager.Instance.QuestEventHandler.QuestStepStateChanged += OnQuestStepStateChanged;

            SaveGameManager.Instance.OnGameDataLoaded += LoadData;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.QuestEventHandler.QuestStarted -= OnQuestStarted;
            GameEventManager.Instance.QuestEventHandler.QuestAdvanced -= OnQuestAdvanced;
            GameEventManager.Instance.QuestEventHandler.QuestFinished -= OnQuestFinished;
            
            GameEventManager.Instance.QuestEventHandler.QuestStepStateChanged -= OnQuestStepStateChanged;
            
            SaveGameManager.Instance.OnGameDataLoaded -= LoadData;
        }

        private void Start()
        {
            foreach (var (id, quest) in _questDictionary)
            {
                quest.QuestDataHandler.SaveData(SaveGameManager.Instance.CurrentGameData);
                GameEventManager.Instance.QuestEventHandler.InvokeQuestStateChanged(quest);
            }
        }
        
        private void Update()
        {
            foreach (var quest in _questDictionary.Values)
            {
                if (quest.State == QuestState.RequirementsNotMet && CheckMeetRequirements(quest))
                {
                    ChangeQuestState(quest.QuestInfoData.ID, QuestState.CanStart);
                }
            }
        }
        
        private void OnQuestStarted(string id)
        {
            var quest = GetQuestById(id);
            quest.InstantiateCurrentQuestStep(this.transform);
            ChangeQuestState(quest.QuestInfoData.ID, QuestState.InProgress);
            quest.QuestDataHandler.SaveData(SaveGameManager.Instance.CurrentGameData);
        }
        
        private void OnQuestAdvanced(string id)
        {
            var quest = GetQuestById(id);
            quest.MoveToNextQuestStep();

            if (quest.CurrentQuestStepExists())
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            else
            {
                ChangeQuestState(quest.QuestInfoData.ID, QuestState.CanFinish);
            }
            
            quest.QuestDataHandler.SaveData(SaveGameManager.Instance.CurrentGameData);
        }
        
        private void OnQuestFinished(string id)
        {
            var quest = GetQuestById(id);
            ClaimRewards(quest);
            RemoveCollectedItems(quest);
            ChangeQuestState(quest.QuestInfoData.ID, QuestState.Finished);
            quest.QuestDataHandler.SaveData(SaveGameManager.Instance.CurrentGameData);
        }
        
        private void OnQuestStepStateChanged(string questId, int questStepIndex, QuestStepState questStepState)
        {
            var quest = GetQuestById(questId);
            quest.StoreQuestStepState(questStepState, questStepIndex);
            ChangeQuestState(questId, quest.State);
        }

        private void LoadData(GameData dataToLoad)
        {
            DestroyAllChildren();
            
            foreach (var quest in _questDictionary.Values)
            {
                quest.QuestDataHandler.LoadData(dataToLoad, this.transform);
                ChangeQuestState(quest.QuestInfoData.ID, quest.State);
            }
        }

        private void DestroyAllChildren()
        {
            int childCount = this.transform.childCount;
            if (childCount == 0) return;

            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = this.transform.GetChild(i).gameObject;
                Destroy(child);
            }
        }
        
        private Dictionary<string, Quest> CreateQuestDictionary()
        {
            var questDictionary = new Dictionary<string, Quest>();
            
            var questInfoArray = Resources.LoadAll<QuestInfoSO>(questInfoDataFolderPath);
            foreach (var questInfo in questInfoArray)
            {
                if (questDictionary.ContainsKey(questInfo.ID))
                    Debug.LogWarning($"QuestManager: {questInfo.ID} already exists in quest dictionary");
                
                questDictionary.Add(questInfo.ID, new Quest(questInfo));
            }
            
            return questDictionary;
        }

        private void ChangeQuestState(string questID, QuestState newState)
        {
            var quest = GetQuestById(questID);
            quest.State = newState;
            
            GameEventManager.Instance.QuestEventHandler.InvokeQuestStateChanged(quest);
        }

        private bool CheckMeetRequirements(Quest quest)
        {
            bool meetRequirements = true;
            
            foreach (var prerequisiteQuestInfoData in quest.QuestInfoData.QuestPrerequisites)
            {
                var prerequisiteQuest = GetQuestById(prerequisiteQuestInfoData.ID);
                if (prerequisiteQuest.State != QuestState.Finished)
                {
                    meetRequirements = false;
                    break;
                }
            }
            
            return meetRequirements;
        }
        
        private void ClaimRewards(Quest quest)
        {
            GameEventManager.Instance.PlayerEventHandler.InvokeGoldGained(quest.QuestInfoData.GoldReward);
        }

        private void RemoveCollectedItems(Quest quest)
        {
            foreach (var questStep in quest.QuestInfoData.QuestSteps)
            {
                if (questStep.TryGetComponent<CollectItemQuestStep>(out var collectStep))
                {
                    GameEventManager.Instance.PlayerEventHandler.InvokeItemRemoved
                    (
                        collectStep.ItemToCollect, 
                        collectStep.AmountRequired
                    );
                }
            }
        }

        private Quest GetQuestById(string id)
        {
            Quest quest = _questDictionary[id];
            if (quest == null)
            {
                Debug.LogError($"QuestManager: {id} doesn't exist in quest dictionary");
            }
            return quest;
        }
    }
}

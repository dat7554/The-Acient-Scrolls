using QuestSystem.Core;
using SaveLoadSystem;
using SaveLoadSystem.Interfaces;
using UnityEngine;

namespace QuestSystem.DataPersistence
{
    public class QuestDataHandler : IDataPersistence
    {
        private readonly Quest _quest;
        private Transform _questManagerTransform;

        public QuestDataHandler(Quest quest)
        {
            _quest = quest;
        }
        
        public void SaveData(GameData gameData)
        {
            var questData = new QuestData(_quest.State, _quest.CurrentQuestStepIndex, _quest.QuestStepStatesArray);
            gameData.questDataDictionary[_quest.QuestInfoData.ID] = questData;
        }

        public void LoadData(GameData gameData)
        {
            if (!gameData.questDataDictionary.TryGetValue(_quest.QuestInfoData.ID, out var questData)) return;
            
            _quest.LoadQuestData(questData);
                
            if (_quest.State is QuestState.InProgress or QuestState.CanFinish)
            {
                _quest.InstantiateCurrentQuestStep(_questManagerTransform);
            }
        }

        public void LoadData(GameData gameData, Transform transform)
        {
            _questManagerTransform = transform;
            LoadData(gameData);
        }
    }
}
using Events;
using QuestSystem.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QuestSystem.UI
{
    public class QuestLogUIController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject questLogUI;
        [SerializeField] private GameObject questDetails;
        [SerializeField] private QuestLogList questLogList;
        [SerializeField] private TextMeshProUGUI questDisplayName;
        [SerializeField] private TextMeshProUGUI questDescription;
        [SerializeField] private TextMeshProUGUI questStep;

        [SerializeField] private Button _firstSelectedSlotUI;
        
        private void OnEnable()
        {
            GameEventManager.Instance.QuestEventHandler.QuestStateChanged += OnQuestStateChanged;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.QuestEventHandler.QuestStateChanged -= OnQuestStateChanged;
        }
        
        public void DisplayQuestLogUI()
        {
            questLogUI.SetActive(true);
            
            if (_firstSelectedSlotUI is null)
            {
                var anyButton = questLogList.GetComponentInChildren<Button>(true);
                if (anyButton is not null) 
                    _firstSelectedSlotUI = anyButton;
            }
            
            questDetails.SetActive(_firstSelectedSlotUI is not null);
            _firstSelectedSlotUI?.Select();
        }

        public void HideQuestLogUI()
        {
            questLogUI.SetActive(false);
            questDetails.SetActive(false);
            
            EventSystem.current.SetSelectedGameObject(null);
        }

        private void OnQuestStateChanged(Quest quest)
        {
            if (quest.State != QuestState.InProgress && quest.State != QuestState.CanFinish)
            {
                _firstSelectedSlotUI = null;
                questLogList.RemoveSlotUI(quest);
                return;
            }
            
            var questLogSlotUI = questLogList.CreateSlotUI(quest, () =>
            {
                SetQuestLogInfo(quest);
            });

            if (_firstSelectedSlotUI is not null || questLogSlotUI is null) return;
            _firstSelectedSlotUI = questLogSlotUI.button;
        }
        
        private void SetQuestLogInfo(Quest quest)
        {
            questDisplayName.text = quest.QuestInfoData.DisplayName;
            questDescription.text = quest.QuestInfoData.Description;
            questStep.text = quest.GetAllStepStateStatus();
        }
    }
}

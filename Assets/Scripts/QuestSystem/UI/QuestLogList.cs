using System.Collections.Generic;
using QuestSystem.Core;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem.UI
{
    public class QuestLogList : MonoBehaviour
    {
        [Header("Content Parent")]
        [SerializeField] private GameObject contentParent;
        
        [Header("Rect Transforms")]
        [SerializeField] private RectTransform scrollRectTransform;
        [SerializeField] private RectTransform contentRectTransform;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject questLogSlotUIPrefab;
        
        private Dictionary<string, QuestLogSlotUI> _questLogSlotUIsByID = new ();

        public QuestLogSlotUI CreateSlotUI(Quest quest, UnityAction selectAction)
        {
            QuestLogSlotUI questLogSlotUI = null;

            if (_questLogSlotUIsByID.TryGetValue(quest.QuestInfoData.ID, out var value))
            {
                questLogSlotUI = value;
            }
            else if (quest.State == QuestState.InProgress)
            {
                questLogSlotUI = InstantiateQuestLogSlotUI(quest, selectAction);
            }
            
            return questLogSlotUI;
        }

        public void RemoveSlotUI(Quest quest)
        {
            if (quest?.QuestInfoData is null) return;

            if (!_questLogSlotUIsByID.Remove(quest.QuestInfoData.ID, out var slot)) return;
            if (slot?.gameObject is not null)
            {
                Destroy(slot.gameObject);
            }
        }
        
        private QuestLogSlotUI InstantiateQuestLogSlotUI(Quest quest, UnityAction selectAction)
        {
            var questLogSlotUI = Instantiate(questLogSlotUIPrefab, contentParent.transform)
                .GetComponent<QuestLogSlotUI>();
            questLogSlotUI.gameObject.name = "SlotUI_" + quest.QuestInfoData.ID;
            RectTransform questLogSlotUIRectTransform = questLogSlotUI.GetComponent<RectTransform>();
            
            questLogSlotUI.Initialize(quest.QuestInfoData.DisplayName, () =>
            {
                selectAction?.Invoke();
                UpdateScrolling(questLogSlotUIRectTransform);
            });
            
            _questLogSlotUIsByID[quest.QuestInfoData.ID] = questLogSlotUI;
            
            return questLogSlotUI;
        }
        
        // TODO: comprehend this
        private void UpdateScrolling(RectTransform questLogSlotUIRectTransform)
        {
            float slotYMin = Mathf.Abs(questLogSlotUIRectTransform.anchoredPosition.y);
            float slotYMax = slotYMin + questLogSlotUIRectTransform.rect.height;

            float contentYMin = contentRectTransform.anchoredPosition.y;
            float contentYMax = contentYMin + scrollRectTransform.rect.height;

            // Scroll down
            if (slotYMax > contentYMax)
            {
                contentRectTransform.anchoredPosition = new Vector2
                    (
                        contentRectTransform.anchoredPosition.x,
                        slotYMax - scrollRectTransform.rect.height
                    );
            }
            // Scroll up
            else if (slotYMin < contentYMin)
            {
                contentRectTransform.anchoredPosition = new Vector2
                    (
                        contentRectTransform.anchoredPosition.x,
                        slotYMin
                    );
            }
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QuestSystem.UI
{
    public class QuestLogSlotUI : MonoBehaviour, ISelectHandler
    {
        public Button button { get; private set; }
        
        [SerializeField] private TextMeshProUGUI displayName;

        private UnityAction _onSelectAction;
        
        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void Initialize(string displayNameText, UnityAction onSelectAction)
        {
            this.displayName.text = displayNameText;
            this._onSelectAction = onSelectAction;
        }

        public void OnSelect(BaseEventData eventData)
        {
            _onSelectAction?.Invoke();
        }
    }
}

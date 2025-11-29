using Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dialogue.UI
{
    public class DialogueChoice : MonoBehaviour, ISelectHandler
    {
        [Header("Components")]
        [SerializeField] private Button choiceButton;
        [SerializeField] private TextMeshProUGUI choiceButtonText;
        
        private int _choiceIndex = -1;
        
        public void SelectButton()
        {
            choiceButton.Select();
        }

        public void SetChoiceText(string text)
        {
            choiceButtonText.text = text;
        }
        
        public void SetChoiceIndex(int index)
        {
            _choiceIndex = index;
        }

        public void OnSelect(BaseEventData eventData)
        {
            GameEventManager.Instance.DialogueEventHandler.InvokeDialogueChoiceIndexUpdated(_choiceIndex);
        }
    }
}

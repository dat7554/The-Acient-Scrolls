using System.Collections.Generic;
using Events;
using Ink.Runtime;
using TMPro;
using UnityEngine;

namespace Dialogue.UI
{
    public class DialogueUIController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject dialogueUI;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private DialogueChoice[] dialogueChoices;

        public bool IsDisplayingDialogueUI => dialogueUI.activeInHierarchy;
        
        private void Awake()
        {
            dialogueUI.SetActive(false);
            ResetUI();
        }

        private void OnEnable()
        {
            GameEventManager.Instance.DialogueEventHandler.DialogueStarted += OnDialogueStarted;
            GameEventManager.Instance.DialogueEventHandler.DialogueFinished += OnDialogueFinished;
            GameEventManager.Instance.DialogueEventHandler.DialogueDisplayed += OnDialogueDisplayed;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.DialogueEventHandler.DialogueStarted -= OnDialogueStarted;
            GameEventManager.Instance.DialogueEventHandler.DialogueFinished -= OnDialogueFinished;
            GameEventManager.Instance.DialogueEventHandler.DialogueDisplayed -= OnDialogueDisplayed;
        }

        public void HideDialogueUI()
        {
            GameEventManager.Instance.InputEventHandler.InvokeConfirmPerformed();
        }

        private void OnDialogueStarted()
        {
            dialogueUI.SetActive(true);
        }

        private void OnDialogueFinished()
        {
            dialogueUI.SetActive(false);
            ResetUI();
        }

        private void OnDialogueDisplayed(string dialogueLine, List<Choice> choices)
        {
            dialogueText.text = dialogueLine;

            if (choices.Count > dialogueChoices.Length)
            {
                Debug.LogError("Too many choices than supported");
            }

            foreach (var dialogueChoice in dialogueChoices)
            {
                dialogueChoice.gameObject.SetActive(false);
            }
            
            int dialogueChoiceIndex = choices.Count - 1;
            for (int index = 0; index < choices.Count; index++)
            {
                var choice = choices[index];
                var dialogueChoice = dialogueChoices[dialogueChoiceIndex];
                
                dialogueChoice.gameObject.SetActive(true);
                dialogueChoice.SetChoiceText(choice.text);
                dialogueChoice.SetChoiceIndex(index);

                if (index == 0)
                {
                    dialogueChoice.SelectButton();
                    GameEventManager.Instance.DialogueEventHandler.InvokeDialogueChoiceIndexUpdated(0);
                }
                
                dialogueChoiceIndex--;
            }
        }

        private void ResetUI()
        {
            dialogueText.text = "";
        }
    }
}

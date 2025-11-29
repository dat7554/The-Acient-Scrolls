using Characters.PlayerSystem.Input;
using Events;
using Ink.Runtime;
using QuestSystem.Core;
using UnityEngine;
using Object = Ink.Runtime.Object;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Ink")]
        [SerializeField] private TextAsset mainInkJSON;
        
        private Story _story;
        private int _currentChoiceIndex = -1;
        
        private bool _isDialogueActive;
        
        private InkExternalFunctions _inkExternalFunctions;
        private InkVariables _inkVariables;

        private void Awake()
        {
            _story = new Story(mainInkJSON.text);
            
            _inkExternalFunctions = new InkExternalFunctions();
            _inkExternalFunctions.Bind(_story);

            _inkVariables = new InkVariables(_story);
        }

        private void OnDestroy()
        {
            _inkExternalFunctions.Unbind(_story);
        }

        private void OnEnable()
        {
            GameEventManager.Instance.InputEventHandler.ConfirmPerformed += HandleConfirmPerformed;
            
            GameEventManager.Instance.DialogueEventHandler.DialogueEntered += HandleDialogueEntered;
            GameEventManager.Instance.DialogueEventHandler.DialogueChoiceIndexUpdated += HandleDialogueChoiceIndexUpdated;
            // GameEventManager.Instance.DialogueEventHandler.InkVariablesUpdated += HandleInkVariablesUpdated;

            GameEventManager.Instance.QuestEventHandler.QuestStateChanged += HandleQuestStateChanged;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.InputEventHandler.ConfirmPerformed -= HandleConfirmPerformed;
            
            GameEventManager.Instance.DialogueEventHandler.DialogueEntered -= HandleDialogueEntered;
            GameEventManager.Instance.DialogueEventHandler.DialogueChoiceIndexUpdated -= HandleDialogueChoiceIndexUpdated;
            // GameEventManager.Instance.DialogueEventHandler.InkVariablesUpdated -= HandleInkVariablesUpdated;
            
            GameEventManager.Instance.QuestEventHandler.QuestStateChanged -= HandleQuestStateChanged;
        }
        
        private void HandleDialogueChoiceIndexUpdated(int index)
        {
            _currentChoiceIndex = index;
        }

        private void HandleConfirmPerformed(InputEventContext context)
        {
            if (!context.Equals(InputEventContext.Dialogue) && !_isDialogueActive) return;
            
            ContinueOrExitDialogue();
        }

        private void HandleQuestStateChanged(Quest quest)
        {
            string variableName = quest.QuestInfoData.ID + "State";
            Object value = new StringValue(quest.State.ToString());
            _inkVariables.UpdateVariablesState(variableName, value);
        }

        private void HandleDialogueEntered(string dialogueKnotName)
        {
            if (_isDialogueActive) return;
            
            _isDialogueActive = true;
            GameEventManager.Instance.DialogueEventHandler.InvokeDialogueStarted();
            GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.UI); // TODO: consider as this has been call in player ui manager
            GameEventManager.Instance.InputEventHandler.ChangeContext(InputEventContext.Dialogue);
            
            if (!dialogueKnotName.Equals(""))
            {
                _story.ChoosePathString(dialogueKnotName);
            }
            else
            {
                Debug.LogWarning("Dialogue knot name is empty");
            }
            
            _inkVariables.SyncVariablesToStory(_story);
            _inkVariables.StartListening(_story);

            ContinueOrExitDialogue();
        }

        // private void HandleInkVariablesUpdated(string variableName, Object value)
        // {
        //     _inkVariables.UpdateVariablesState(variableName, value);
        // }

        private void ContinueOrExitDialogue()
        {
            if (_story.currentChoices.Count > 0 && _currentChoiceIndex != -1)
            {
                _story.ChooseChoiceIndex(_currentChoiceIndex);
                _currentChoiceIndex = -1;
            }
            
            if (_story.canContinue)
            {
                string dialogueLine = _story.Continue();

                while (IsDialogueLineBlank(dialogueLine) && _story.canContinue)
                {
                    dialogueLine = _story.Continue();
                }

                if (IsDialogueLineBlank(dialogueLine) && !_story.canContinue)
                {
                    ExitDialogue();
                }
                else
                {
                    GameEventManager.Instance.DialogueEventHandler.InvokeDialogueDisplayed(dialogueLine, _story.currentChoices); 
                }
            }
            else if (_story.currentChoices.Count == 0)
            {
                ExitDialogue();
            }
        }

        private void ExitDialogue()
        {
            _isDialogueActive = false;
            GameEventManager.Instance.DialogueEventHandler.InvokeDialogueFinished();
            GameEventManager.Instance.UIEventHandler.InvokeActionMapChangeRequested(ActionMap.Player); // TODO: consider as this has been call in player ui manager
            GameEventManager.Instance.InputEventHandler.ChangeContext(InputEventContext.Default);
            
            _inkVariables.StopListening(_story);
            
            _story.ResetState();
        }

        private bool IsDialogueLineBlank(string dialogueLine)
        {
            return string.IsNullOrEmpty(dialogueLine);
        }
    }
}

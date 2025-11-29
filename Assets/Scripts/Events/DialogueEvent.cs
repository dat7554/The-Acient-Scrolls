using System;
using System.Collections.Generic;
using Ink.Runtime;
using Object = Ink.Runtime.Object;

namespace Events
{
    public class DialogueEvent
    {
        #region Events
        
        public event Action<string> DialogueEntered;
        
        public event Action DialogueStarted;
        public event Action DialogueFinished;
        public event Action<string, List<Choice>> DialogueDisplayed;
        
        public event Action<int> DialogueChoiceIndexUpdated;
        
        // public event Action<string, Object> InkVariablesUpdated;
        
        #endregion
        
        #region Event invocation methods
        
        public void InvokeDialogueEntered(string knotName) => DialogueEntered?.Invoke(knotName);
        
        public void InvokeDialogueStarted() => DialogueStarted?.Invoke();
        public void InvokeDialogueFinished() => DialogueFinished?.Invoke();
        public void InvokeDialogueDisplayed(string dialogueLine, List<Choice> dialogueChoices) 
            => DialogueDisplayed?.Invoke(dialogueLine, dialogueChoices);
        
        public void InvokeDialogueChoiceIndexUpdated(int index) => DialogueChoiceIndexUpdated?.Invoke(index);
        
        // public void InvokeInkVariablesUpdated(string name, Object value) => InkVariablesUpdated?.Invoke(name, value);
        
        #endregion
    }
}

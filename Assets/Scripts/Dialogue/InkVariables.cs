using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using Object = Ink.Runtime.Object;

namespace Dialogue
{
    /// <summary>
    /// The bridge between the game's state and Ink's dialogue state,
    /// ensuring that variables persist across different dialogue sessions
    /// </summary>
    public class InkVariables
    {
        private Dictionary<string, Object> _variables;

        public InkVariables(Story story)
        {
            _variables = new Dictionary<string, Object>();

            foreach (var name in story.variablesState)
            {
                Object value = story.variablesState.GetVariableWithName(name);
                _variables.Add(name, value);
                Debug.Log("Initialized global variable: " + name + " = " + value);
            }
        }

        public void SyncVariablesToStory(Story story)
        {
            foreach (KeyValuePair<string, Object> variable in _variables)
            {
                story.variablesState.SetGlobal(variable.Key, variable.Value);
            }
        }

        public void StartListening(Story story)
        {
            story.variablesState.variableChangedEvent += UpdateVariablesState;
        }

        public void StopListening(Story story)
        {
            story.variablesState.variableChangedEvent -= UpdateVariablesState;
        }

        public void UpdateVariablesState(string name, Object value)
        {
            if (_variables.ContainsKey(name))
            {
                _variables[name] = value;
                Debug.Log("Updated variable: " + name + " = " + value);
            }
        }
    }
}

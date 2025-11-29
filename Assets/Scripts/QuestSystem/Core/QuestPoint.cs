using Events;
using UnityEngine;

namespace QuestSystem.Core
{
    public class QuestPoint : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private string dialogueKnotName;
        
        public void EnterDialogueSubmitted()
        {
            if (!dialogueKnotName.Equals(""))
            {
                GameEventManager.Instance.DialogueEventHandler.InvokeDialogueEntered(dialogueKnotName);
            }
        }
    }
}

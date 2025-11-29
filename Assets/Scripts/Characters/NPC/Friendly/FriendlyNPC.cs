using Characters.PlayerSystem;
using Items.Core.Interfaces;
using QuestSystem.Core;
using UnityEngine;

namespace Characters.NPC.Friendly
{
    public class FriendlyNPC : MonoBehaviour, IInteractable
    {
        [SerializeField] protected string interactionPrompt;
        
        private QuestPoint _questPoint;
        private FriendlyNPCAnimator _animator;

        private void Start()
        {
            _questPoint = GetComponent<QuestPoint>();
            _animator = GetComponent<FriendlyNPCAnimator>();
        }

        public virtual string GetInteractionPrompt() => interactionPrompt;

        public virtual void HandleInteract(PlayerInteract playerInteract)
        {
            DisplayDialogues();
            StartCoroutine(_animator.RotateTowardsTarget(playerInteract.Character));
        }

        private void DisplayDialogues()
        {
            _questPoint?.EnterDialogueSubmitted();
        }
    }
}

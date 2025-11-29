using Characters.PlayerSystem;
using Items.Core.Interfaces;
using UnityEngine;

public class GrabbaleCube : MonoBehaviour, IInteractable, IGrabbable
{
    [Header("Prompt")]
    [SerializeField] private string interactionPrompt;
    
    public string GetInteractionPrompt() => interactionPrompt;

    public void HandleInteract(PlayerInteract playerInteract)
    {
        
    }
}

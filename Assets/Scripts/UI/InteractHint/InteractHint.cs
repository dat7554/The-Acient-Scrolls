using Items.Core.Interfaces;
using TMPro;
using UnityEngine;

public class InteractHint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactPrompt;

    public void DisplayPrompt(IInteractable interactableObject)
    {
        interactPrompt.text = interactableObject.GetInteractionPrompt();
    }
}
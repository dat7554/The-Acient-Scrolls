using Characters.PlayerSystem;

namespace Items.Core.Interfaces
{
    public interface IInteractable
    {
        string GetInteractionPrompt();

        void HandleInteract(PlayerInteract playerInteract);
    }
}

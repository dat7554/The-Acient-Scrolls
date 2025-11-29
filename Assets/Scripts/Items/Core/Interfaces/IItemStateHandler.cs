using Items.Components.Weapons;

namespace Items.Core.Interfaces
{
    // TODO: consider to be abstract class
    public interface IItemStateHandler
    {
        void OnEquip();
        void OnDrop();
        void SetVisibility(ItemVisibilityMode mode);
    }
}
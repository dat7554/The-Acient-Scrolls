using System;
using Characters.PlayerSystem;
using PlayerSystem;
using UnityEngine;

namespace InventorySystem.UI
{
    public class HotbarUI : StaticInventoryUI
    {
        private int _maxIndex = 5;
        private int _currentIndex = 0;
        
        private PlayerInventoryHolder _playerInventoryHolder;

        protected override void Start()
        {
            base.Start();
            
            _maxIndex = slotUIArray.Length - 1;
            _currentIndex = 0;
            
            _playerInventoryHolder = holder as PlayerInventoryHolder;
        }

        public void Selected(int index)
        {
            switch (index)
            {
                case 0:
                    SetIndex(0);
                    break;
                case 1:
                    SetIndex(1);
                    break;
                case 2:
                    SetIndex(2);
                    break;
                case 3:
                    SetIndex(3);
                    break;
                case 4:
                    SetIndex(4);
                    break;
                case 5:
                    SetIndex(5);
                    break;
            }
        }

        private void SetIndex(int newIndex)
        {
            if (newIndex < 0) newIndex = 0;
            else if (newIndex > _maxIndex) newIndex = _maxIndex;
            
            _currentIndex = newIndex;
            var itemDataOnIndex = slotUIArray[_currentIndex].AssignedInventorySlot.ItemData;
            if (itemDataOnIndex is not null)
                _playerInventoryHolder?.UseItem(itemDataOnIndex);
        }
    }
}
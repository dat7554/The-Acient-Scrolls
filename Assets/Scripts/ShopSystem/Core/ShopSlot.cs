using System;
using Items.Core;
using UnityEngine;

namespace ShopSystem.Core
{
    [Serializable]
    public class ShopSlot : ItemSlot
    {
        public ShopSlot()
        {
            ClearSlot();
        }
    }
}
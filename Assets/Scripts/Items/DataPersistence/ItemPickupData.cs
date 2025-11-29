using System;
using UnityEngine;

namespace Items.DataPersistence
{
    [Serializable]
    public struct ItemPickupData
    {
        public int itemID;
        public Vector3 position;
        public Quaternion rotation;
    }
}
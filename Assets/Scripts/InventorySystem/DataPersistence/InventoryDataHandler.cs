using SaveLoadSystem;
using SaveLoadSystem.Interfaces;
using UnityEngine;

namespace InventorySystem.DataPersistence
{
    public abstract class InventoryDataHandler : MonoBehaviour, IDataPersistence
    {
        public abstract void SaveData(GameData gameData);
        public abstract void LoadData(GameData gameData);
    }
}
using System;
using System.Collections.Generic;
using Characters.NPC.DataPersistence;
using Characters.PlayerSystem.DataPersistence;
using InventorySystem.DataPersistence;
using Items.DataPersistence;
using QuestSystem.DataPersistence;
using ShopSystem.DataPersistence;
using Utilities.Collections;

namespace SaveLoadSystem
{
    [Serializable]
    public class GameData
    {
        public string dateTimeISO;
        
        public PlayerData playerData;
        
        public List<string> collectedItems = new ();
        public SerializableDictionary<string, ItemPickupData> activeItems = new ();
        
        public SerializableDictionary<string, InventoryData> chestDataDictionary = new ();
        
        public SerializableDictionary<string, ShopData> shopDataDictionary = new ();
        
        public SerializableDictionary<string, QuestData> questDataDictionary = new ();

        public SerializableDictionary<string, EnemySpawnerData> enemySpawnerDataDictionary = new ();
        
        public SerializableDictionary<string, EnemyNPCData> enemyDataDictionary = new ();
    }
}
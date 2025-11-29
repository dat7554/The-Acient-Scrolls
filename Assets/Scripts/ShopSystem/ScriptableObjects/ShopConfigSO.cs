using System;
using System.Collections.Generic;
using Items.ScriptableObjects;
using UnityEngine;

namespace ShopSystem.ScriptableObjects
{
    [Serializable]
    public struct ShopEntry
    {
        public ItemSO itemData;
        public int quantity;
    }
    
    [CreateAssetMenu(fileName = "ShopConfig", menuName = "ShopSystem/ShopConfig")]
    public class ShopConfigSO : ScriptableObject
    {
        [SerializeField] private List<ShopEntry> items = new List<ShopEntry>();
        [SerializeField] private int maxGold;
        [SerializeField] private float playerSellMarkUp;
        [SerializeField] private float playerBuyMarkUp;
        
        public List<ShopEntry> Items => items;
        public int MaxGold => maxGold;
        public float PlayerSellMarkUp => playerSellMarkUp;
        public float PlayerBuyMarkUp => playerBuyMarkUp;
    }
}

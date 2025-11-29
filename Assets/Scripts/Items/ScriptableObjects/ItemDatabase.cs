
using System.Collections.Generic;
using System.Linq;
using Items.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace InventorySystem.ScriptableObjects
{
    // TODO: consider move this to inventory system
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "InventorySystem/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private string itemDataFolderPath = "Items/ItemData";
        [SerializeField] private List<ItemSO> itemDatabase;

        // TODO: refactor
        [ContextMenu("Set IDs")]
        public void SetItemIDsInItemDatabase()
        {
            itemDatabase = new List<ItemSO>();
            
            var foundItems = Resources.LoadAll<ItemSO>(itemDataFolderPath)
                .OrderBy(i => i.ItemID)
                .ToList();

            var hasIDInRange = foundItems
                .Where(i => i.ItemID != -1 && i.ItemID < foundItems.Count)
                .OrderBy(i => i.ItemID)
                .ToList();
            var hasIDNotInRange = foundItems
                .Where(i => i.ItemID != -1 && i.ItemID >= foundItems.Count)
                .OrderBy(i => i.ItemID)
                .ToList();
            var noID = foundItems
                .Where(i => i.ItemID <= -1)
                .ToList();

            var tempIndex = 0;
            for (int index = 0; index < foundItems.Count; index++)
            {
                var itemDataToAdd = hasIDInRange.Find(itemData => itemData.ItemID == index);

                if (itemDataToAdd != null)
                {
                    itemDatabase.Add(itemDataToAdd);
                }
                else if (tempIndex < noID.Count)
                {
                    noID[tempIndex].SetItemID(index);
                    itemDataToAdd = noID[tempIndex];
                    tempIndex++;
                    itemDatabase.Add(itemDataToAdd);
                }

                #if UNITY_EDITOR
                if (itemDataToAdd) 
                    EditorUtility.SetDirty(itemDataToAdd);
                #endif
            }

            foreach (var itemData in hasIDNotInRange)
            {
                itemDatabase.Add(itemData);
                
                #if UNITY_EDITOR
                if (itemData) EditorUtility.SetDirty(itemData);
                #endif
            }
            
            #if UNITY_EDITOR       
            AssetDatabase.SaveAssets();
            #endif
        }

        public ItemSO GetItemDataFromItemDatabase(int itemID)
        {
            return itemDatabase.Find(i => i.ItemID == itemID);
        }
    }
}

using System;
using UnityEngine;
using Utilities.Collections;

namespace SaveLoadSystem
{
    [Serializable]
    public class UniqueIdGenerator : MonoBehaviour
    {
        private static SerializableDictionary<string, GameObject> _idDatabase = 
            new SerializableDictionary<string, GameObject>();
        
        [ReadOnly, SerializeField] private string id;
        
        public string ID => id;
        public bool IsRuntimeInstance { get; private set; }

        private void Awake()
        {
            if (_idDatabase is null)
                _idDatabase = new SerializableDictionary<string, GameObject>();
            
            // Detect runtime vs scene
            IsRuntimeInstance = string.IsNullOrEmpty(id);
            if (IsRuntimeInstance || _idDatabase.ContainsKey(this.id))
                Generate();
            else
                _idDatabase.TryAdd(this.id, this.gameObject);
        }
        
        public void ForceSetID(string newId)
        {
            if (!IsRuntimeInstance)
                return;
            
            _idDatabase.Remove(this.id);

            // Remove the old key (if one was generated in Awake)
            this.id = newId;
            _idDatabase[this.id] = gameObject;
        }
        
        public static bool IsRegistered(string id)
        {
            return _idDatabase != null && _idDatabase.ContainsKey(id);
        }

        private void OnDestroy()
        {
            _idDatabase.Remove(this.id);
        }

        [ContextMenu("Generate ID")]
        private void Generate()
        {
            this.id = Guid.NewGuid().ToString();
            _idDatabase.Add(this.id, this.gameObject);
            
            // Debug purpose only
            Debug.Log($"ID database count: {_idDatabase.Count}");
        }
    }
}

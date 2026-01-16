using System.Collections.Generic;
using Resources.Script.Managers;
using UnityEngine;

namespace Resources.Script
{
    public class ScriptableObjCatalog : MonoBehaviour
    {
        [SerializeField] private List<ObjectPreset> _soList;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        
        private Dictionary<string, ObjectPreset> _soDict = new();

        void Awake()
        {
            foreach (var so in _soList)
            {
                _soDict.Add(so.prefab.name, so);
            }
        }

        public GameObject GetPrefab(string key)
        {
            if (!_soDict.TryGetValue(key, out var value))
            {
                Debug.LogError($"{key} not found in ScriptableObjCatalog Dictionary");
                return null;
            }

            return value.prefab;
        }
        
        public ObjectPreset GetObjPreset(string key)
        {
            if (!_soDict.TryGetValue(key, out var value))
            {
                Debug.LogError($"{key} not found in ScriptableObjCatalog Dictionary");
                return null;
            }

            return value;
        }
    }
}

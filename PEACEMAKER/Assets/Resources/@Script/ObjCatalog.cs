using System.Collections.Generic;
using Resources.Script.Managers;
using UnityEngine;

namespace Resources.Script
{
    public class ObjCatalog : MonoBehaviour
    {
        [SerializeField] private List<ObjectPreset> _soList;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        
        private Dictionary<Defines.EObjectID, ObjectPreset> _soDict = new();

        void Awake()
        {
            foreach (var so in _soList)
            {
                _soDict.Add(so.id, so);
            }
        }

        public GameObject GetPrefab(Defines.EObjectID key)
        {
            if (!_soDict.TryGetValue(key, out var value))
            {
                Debug.LogError($"{key} not found in ScriptableObjCatalog Dictionary");
                return null;
            }

            return value.prefab;
        }
        
        public ObjectPreset GetObjPreset(Defines.EObjectID key)
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

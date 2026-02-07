using System;
using System.Collections.Generic;
using Resources.Script.Ability;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Resources.Script
{
    public class AbilityPool : MonoBehaviour
    {
        [SerializeField] private List<AbilityDef> _abilityList;
        private Dictionary<int, AbilityDef> _abilityDictionary = new();
        
        public List<AbilityDef> AbilityList => _abilityList;
        public Dictionary<int, AbilityDef> AbilityDictionary => _abilityDictionary;
        private void Awake()
        {
            Init();
        }
        
        public void Init()
        {
            _abilityDictionary.Clear();
            foreach (var abilityDef in _abilityList)
            {
                _abilityDictionary.Add(abilityDef.id, Instantiate(abilityDef));
            }
        }
        
        public AbilityDef GetAbility(int key)
        {
            if (!_abilityDictionary.TryGetValue(key, out var value))
            {
                Debug.LogError($"{key} not found in ScriptableObjCatalog Dictionary");
                return null;
            }

            return value;
        }
    }
}
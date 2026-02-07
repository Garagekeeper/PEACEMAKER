using System.Collections.Generic;
using System.Linq;
using Resources.Script.Ability;
using UnityEngine;
using static Resources.Script.Utilities;

namespace Resources.Script.Managers
{
    public class AbilityManager
    {
        private AbilityPool _abilityPool;
        public AbilityPool AbilityPool { get => _abilityPool; private set =>  _abilityPool = value; }

        
        public AbilityManager(AbilityPool  abilityPool)
        {
            _abilityPool = abilityPool;
            if (_abilityPool.AbilityList.Count == 0) 
                Debug.LogError("Ability pool contains no abilities");
        }

        public void Init()
        {
            AbilityPool.Init();
        }
        
        public List<AbilityDef> SelectThreeAbilities()
        {
            List<int> abilityKeyList = new();
            // 최대 레벨에 도달하지 않은 능력들만 골라오기
            foreach (var abilityDefDict in AbilityPool.AbilityDictionary)
            {
                if (abilityDefDict.Value.IsCapped()) continue;
                abilityKeyList.Add(abilityDefDict.Key);
            }

            if (abilityKeyList.Count < 3)
            {
                Debug.LogError("[Ability pool] not enough abilities. (must larger than 3)");
                return null;
            }
            
            Shuffle(abilityKeyList);
            
            List<AbilityDef> result = new List<AbilityDef>();
            foreach (var abilityKey in abilityKeyList.GetRange(0,3))
            {
                result.Add(AbilityPool.GetAbility(abilityKey));
            }

            return result;

        }
    }
}
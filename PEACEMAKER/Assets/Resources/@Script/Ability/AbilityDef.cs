using System.Collections.Generic;
using Resources.Script.Creatures;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Ability
{
    [CreateAssetMenu(fileName = "New Normal Ability", menuName = "PEACEMAKER/Ability/Normal")]
    public class AbilityDef : ScriptableObject
    { 
        [Header("key")]
        public int id;

        [Header("UI")] 
        public Sprite icon;
        public string title;
        [TextArea] public string description;
        public int weight = 10;
        
        [Header("Value with level")]
        [Tooltip("먼저 생성된거 : 낮은 레벨, 나중에 생성된거 : 높은 레벨")]
        public List<float> Values = new List<float>();

        [Header("Target")] public EAbilityTarget target;
        [Header("Operator")] public EOperator op;
        
        private int _level = 0;
        public int AbilityLevel => _level;
        [SerializeField]private int _maxLevel = 990;
        public int MaxLevel => _maxLevel;
        protected Dictionary<int, float> abilityDict = new Dictionary<int, float>();

        private void OnEnable()
        {
            for (var i = 0; i < Values.Count; i++)
            {
                abilityDict[i] = Values[i];
            }
        }

        /// <summary>
        /// 상한선에 도달했는지 확인
        /// </summary>
        /// <returns></returns>
        public bool IsCapped()
        {
            return _level >= _maxLevel;
        }

        /// <summary>
        /// 기본 값에다 레벨에 따른 배율을 더해서 적용할 값 산출
        /// </summary>
        /// <returns></returns>
        public float GetNextValue()
        {
            return abilityDict[_level];
        }

        /// <summary>
        /// 산출한 값을 설명칸에 작성
        /// </summary>
        /// <returns></returns>
        public virtual string GetDescription()
        {
            return string.Format(description, GetNextValue());
        }

        /// <summary>
        /// Title영역에 작성될 문자열을 반환
        /// title {number} LV
        /// </summary>
        /// <returns></returns>
        public virtual string GetTitle()
        {
            string res = (_level + 1).ToString();
            if (_level == _maxLevel - 1)
                res = "MAX";
            
            return title +" "+ res + " LV";
        }

        /// <summary>
        /// 실제 값 적용
        /// </summary>
        public virtual float GetFinalValue()
        {
            // 캡이면 적용 안 함(원하면 다른 처리)
            if (_level >= _maxLevel)
            {
                Debug.LogError("[AbilityDef] Max level ability selected");
                return 0; 
            }
            var val = GetNextValue();
            
            _level++;
            return val;
        }

    }
}
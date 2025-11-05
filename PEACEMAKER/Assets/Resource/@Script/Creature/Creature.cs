using System;
using UnityEngine;
using UnityEngine.Events;
using static Resource.Script.Defines;

namespace Resource.Script.Creature
{
    /// <summary>
    /// 모든 생명체의 기본 클래스
    /// </summary>
    public class Creature : MonoBehaviour
    {
        public  ECreatureType CreatureType { get; protected set; }
   
        protected long Kills { get; set; }
        
        protected virtual void Awake()
        {
            Kills = 0;
        }

        protected virtual void Update()
        {
            
        }
        
        public void GetKill()
        {
            Kills += 1;
        }

        protected void OnDestroy()
        {
            
        }
    }

    
}

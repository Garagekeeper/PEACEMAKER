using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Creatures
{
    /// <summary>
    /// 모든 생명체의 기본 클래스
    /// </summary>
    public class Creature : BaseObject
    {
        public ECreatureType CreatureType { get; protected set; }
        public bool IsInvincible;

        public Transform dropTransform;
        
        public int CreatureLevel { get;  protected set; }
   
        protected long KillCounts { get; set; }
        
        protected virtual void Awake()
        {
            KillCounts = 0;
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            
        }
        
        public virtual void GetKill()
        {
            KillCounts += 1;
        }

        protected void OnDestroy()
        {
            
        }
    }

    
}

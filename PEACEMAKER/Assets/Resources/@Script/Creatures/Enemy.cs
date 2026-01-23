using Resources.Script.Controller;
using Resources.Script.InteractiveObject;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using static Resources.Script.Defines;

namespace Resources.Script.Creatures
{
    public class Enemy : DamageableObject, IMovable
    {
        

        public float Speed { get; set; } = 3f;
        public float SpeedMultiplier { get; set; } = 1f;
        
        public ERarity Rarity { get; set; }
        private EnemyController  _controller;
        public EObjectID GemID { get; set; }
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Enemy;
            _controller = GetComponent<EnemyController>();
            Rarity = ERarity.Normal;
            GemID = EObjectID.ExpGemNormal + (int)Rarity;
            ObjectType = EObjectType.Enemy;
        }

        public override void OnDeath()
        {
            base.OnDeath();
            _controller.OnDead();
            GetComponent<CharacterController>().enabled = false;
            
            // spawn EXP gem
            
            HeadManager.ObjManager.Spawn<ExpGem>(GemID, dropTransform.position);
            
            Destroy(gameObject, 5);
        }

    }
}
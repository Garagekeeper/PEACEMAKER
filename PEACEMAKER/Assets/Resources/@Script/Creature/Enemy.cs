using Resources.Script.Controller;
using UnityEngine;
using static Resources.Script.Defines;
namespace Resources.Script.Creature
{
    public class Enemy : DamageableObject
    {
        private EnemyController  _controller;
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Enemy;
            _controller = GetComponent<EnemyController>();
        }

        public override void OnDeath()
        {
            base.OnDeath();
            _controller.OnDead();
            Destroy(gameObject, 5);
        }
    }
}
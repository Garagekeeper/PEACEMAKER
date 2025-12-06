using Resources.Script.Managers;
using UnityEditor;
using UnityEngine;
using static Resources.Script.Defines;
namespace Resources.Script.Creature
{
    public class Player : DamageableObject
    {
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Player;
            SystemManager.Game.MainPlayer = this;
        }

        public override void OnDeath()
        {
            Debug.Log("Player Death");
        }

        public override void OnDamage(float value, Creature attackBy)
        {
            SystemManager.UI.HpEffect.TriggerDamageEffect();
            base.OnDamage(value, attackBy);
        }
        
    }
}
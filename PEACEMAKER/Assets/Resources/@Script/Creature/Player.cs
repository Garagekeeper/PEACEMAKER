using Resources.Script.Managers;
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
        
    }
}
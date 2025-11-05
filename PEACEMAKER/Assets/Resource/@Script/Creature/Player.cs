using UnityEngine;
using UnityEngine.Events;
using static Resource.Script.Defines;
namespace Resource.Script.Creature
{
    public class Player : DamageableCreature
    {
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Player;
        }

        protected override void HandleDeath(Creature attackBy)
        {
            base.HandleDeath(attackBy);
        }
        
    }
}
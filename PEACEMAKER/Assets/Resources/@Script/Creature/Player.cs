using static Resources.Script.Defines;
namespace Resources.Script.Creature
{
    public class Player : DamageableObject
    {
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Player;
        }

        protected override void HandleDeath(Resources.Script.Creature.Creature attackBy)
        {
            base.HandleDeath(attackBy);
        }
        
    }
}
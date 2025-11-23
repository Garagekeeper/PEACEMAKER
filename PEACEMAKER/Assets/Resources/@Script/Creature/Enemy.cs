using static Resources.Script.Defines;
namespace Resources.Script.Creature
{
    public class Enemy : DamageableCreature
    {
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Enemy;
        }

        protected override void HandleDeath(Resources.Script.Creature.Creature attackBy)
        {
            base.HandleDeath(attackBy);
        }
    }
}
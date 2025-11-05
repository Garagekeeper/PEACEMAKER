using static Resource.Script.Defines;
namespace Resource.Script.Creature
{
    public class Enemy : DamageableCreature
    {
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Enemy;
        }

        protected override void HandleDeath(Creature attackBy)
        {
            base.HandleDeath(attackBy);
        }
    }
}
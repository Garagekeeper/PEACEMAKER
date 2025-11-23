using static Resources.Script.Defines;

namespace Resources.Script.Creature
{
    public class NPC : Resources.Script.Creature.Creature
    {
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Npc;
        }
    }
}
using static Resources.Script.Defines;

namespace Resources.Script.Creatures
{
    public class NPC : Creature
    {
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Npc;
        }
    }
}
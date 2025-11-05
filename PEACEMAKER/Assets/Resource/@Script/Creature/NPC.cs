using static Resource.Script.Defines;

namespace Resource.Script.Creature
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
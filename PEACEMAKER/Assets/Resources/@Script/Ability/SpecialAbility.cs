using Resources.Script.Creatures;
using UnityEngine;

namespace Resources.Script.Ability
{
    public class SpecialAbility : AbilityDef
    {
        public override float GetFinalValue()
        {
            Debug.Log("Default Apply Internal");
            return 0;
        }
    }
}
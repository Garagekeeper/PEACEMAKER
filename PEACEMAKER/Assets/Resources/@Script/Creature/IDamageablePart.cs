using UnityEngine;

namespace Resources.Script.Creature
{
    public interface IDamageablePart
    {
        public IDamageable ParentDamageable { get; }
        public HumanBodyBones Bone { get; }
        public string PartName { get; }

        public float DamageMultiplier { get; }
        public bool IsCriticalPart { get; }
    }
}
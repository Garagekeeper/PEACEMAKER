using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resource.Script.Animation.Modifier
{
    [AddComponentMenu("PEACEMAKER/Animation/Modifiers/Offset Animation Modifier")]
    public class OffsetAnimationModifier : ProceduralAnimationModifier
    {
        [Tooltip("final position result for this modifier")]
        public Vector3 positonOffset;
        [Tooltip("final rotation result for this modifier")]
        public Vector3 rotationOffset;

        private void Update()
        {
            TargetPosition = positonOffset;
            TargetRotation = rotationOffset;
        }
    }
}
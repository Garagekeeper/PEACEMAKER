using UnityEngine;

namespace Resource.Script.Animation.Modifier
{
    [AddComponentMenu("PEACEMAKER/Animation/Modifiers/Move Animation Modifier"), RequireComponent(typeof(ProceduralAnimation))]
    public class MoveAnimationModifier : ProceduralAnimationModifier
    {
        public Vector3 position;
        public Vector3 rotation;

        public Vector3 defaultPosition;
        public Vector3 defaultRotation;

        protected void Update()
        {
            TargetPosition = Vector3.Lerp(defaultPosition, position, TargetAnimation.Progress);
            TargetRotation = Vector3.Lerp(defaultRotation, rotation, TargetAnimation.Progress);
        }
    }
}
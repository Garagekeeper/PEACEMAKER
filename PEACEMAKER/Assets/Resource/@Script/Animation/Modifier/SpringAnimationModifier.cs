using UnityEngine;
using static Resource.Script.Defines;

namespace Resource.Script.Animation.Modifier
{
    /// <summary>
    /// 스프링은 부드러운 반복운동
    /// </summary>
    [AddComponentMenu("PEACEMAKER/Animation/Modifiers/Spring Animation Modifier"), RequireComponent(typeof(ProceduralAnimation))]
    public class SpringAnimationModifier : ProceduralAnimationModifier
    {
        public float speed = 1;
        public SpringVector3 position = new SpringVector3();
        public SpringVector3 rotation = new SpringVector3();

        private void Update()
        {
            position.Update(speed * GlobalSpeed);
            rotation.Update(speed * GlobalSpeed);

            TargetPosition = position.result;
            TargetRotation = rotation.result;
        }

        public void Trigger()
        {
            position.Start(position.value);
            rotation.Start(rotation.value);
        }

        public void Trigger(Vector3 position, Vector3 rotation)
        {
            print(position);
            this.position.Start(position);
            this.rotation.Start(rotation);
        }
    }
    
    [System.Serializable]
    public class SpringVector3
    {
        public Vector3 value;
        public float speed = 10;
        public float fadeOutTime = 1;
        [Range(0, 1)] public float weight = 1;

        public float time { get; set; }
        public float progress { get; set; }
        public Vector3 result { get; set; }
        private float velocity;

        public void Update(float globalSpeed)
        {
            time += Time.deltaTime * speed * globalSpeed;

            float pos = 0;

            pos += Mathf.Sin(time) * progress;

            progress = Mathf.SmoothDamp(progress, 0, ref velocity, fadeOutTime / globalSpeed);

            result = new Vector3(pos * value.x, pos * value.y, pos * value.z) * progress;
        }

        public void Start(Vector3 value)
        {
            this.value = value;
            progress = 1 * weight;
            time = 0;
        }
    }
}
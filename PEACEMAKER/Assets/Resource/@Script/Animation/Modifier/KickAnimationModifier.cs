using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static Resource.Script.Defines;

namespace Resource.Script.Animation.Modifier
{
    /// <summary>
    /// 무기 발사나 반동(Kick) 시 캐릭터 손, 무기 등의 
    /// 위치/회전을 ProceduralAnimation을 통해 흔들리게 만드는 Modifier
    /// 부르드러누 반동이 아니라 급격한 흔들림
    /// </summary>
    [AddComponentMenu("PEACEMAKER/Animation/Modifiers/Kick Animation Modifier")]
    public class KickAnimationModifier : ProceduralAnimationModifier
    {
        public EUpdateMode updateMode = EUpdateMode.FixedUpdate;
        /// <summary>
        /// 위치 변화 가중치
        /// </summary>
        [Range(0, 1)] public float positionWeight = 1;
        /// <summary>
        /// 회전 변화 가중치
        /// </summary>
        [Range(0, 1)] public float rotationWeight = 1;
        /// <summary>
        /// 위치보간 기울기 (크면 빠르게 근접)
        /// </summary>
        public float positionRoughness = 10;
        /// <summary>
        /// 회전 보간 가중치 (크면 빠르게 근접)
        /// </summary>
        public float rotationRoughness = 10;
        /// <summary>
        /// 위치 변환 시 고정적으로 추가되는 값
        /// </summary>
        public Vector3 staticPosition;
        /// <summary>
        /// 회전시 고정적으로 추가되는 값
        /// </summary>
        public Vector3 staticRotation;
        /// <summary>
        /// 위치 변환 시 랜덤으로 추가되는 값
        /// </summary>
        public Vector3 randomPosition;
        /// <summary>
        /// 회전시 랜덤으로 추가되는 값
        /// </summary>
        public Vector3 randomRotation;

        [Space]
        public UnityEvent OnTrigger = new UnityEvent();

        private Vector3 _currentRotation;
        private Vector3 _currentPosition;

        private void Update()
        {
            if (updateMode == EUpdateMode.Update)
            {
                float deltaTime = Time.deltaTime * GlobalSpeed; // Scale time

                TargetPosition = Vector3.Slerp(TargetPosition, _currentPosition, positionRoughness * deltaTime) * (positionWeight * GlobalSpeed);
                TargetRotation = Vector3.Slerp(TargetRotation, _currentRotation, rotationRoughness * deltaTime) * (rotationWeight * GlobalSpeed);
            }
        }

        private void FixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime * GlobalSpeed; // Scale time

            // 반동 복구
            _currentPosition = Vector3.Lerp(_currentPosition, Vector3.zero, 35 * fixedDeltaTime) * GlobalSpeed;
            _currentRotation = Vector3.Lerp(_currentRotation, Vector3.zero, 35 * fixedDeltaTime) * GlobalSpeed;

            if (updateMode == EUpdateMode.FixedUpdate)
            {
                // 목표 위치/회전으로 보간
                TargetPosition = Vector3.Slerp(TargetPosition, _currentPosition, positionRoughness * fixedDeltaTime) * (positionWeight * GlobalSpeed);
                TargetRotation = Vector3.Slerp(TargetRotation, _currentRotation, rotationRoughness * fixedDeltaTime) * (rotationWeight * GlobalSpeed);
            }
        }

        private void LateUpdate()
        {
            if (updateMode == EUpdateMode.LateUpdate)
            {
                float deltaTime = Time.deltaTime * GlobalSpeed; // Scale time

                TargetPosition = Vector3.Slerp(TargetPosition, _currentPosition, positionRoughness * deltaTime) * (positionWeight * GlobalSpeed);
                TargetRotation = Vector3.Slerp(TargetRotation, _currentRotation, rotationRoughness * deltaTime) * (rotationWeight * GlobalSpeed);
            }
        }


        public void Trigger()
        {
            _currentPosition += staticPosition + new Vector3(Random.Range(randomPosition.x, -randomPosition.x), Random.Range(randomPosition.y, -randomPosition.y), randomPosition.z) * GlobalSpeed;
            _currentRotation += staticRotation + new Vector3(randomRotation.x, Random.Range(randomRotation.y, -randomRotation.y), Random.Range(randomRotation.z, -randomRotation.z)) * GlobalSpeed;
            OnTrigger?.Invoke();
        }
    }
}
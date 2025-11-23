using System;
using UnityEngine;

namespace Resources.Script.Animation.Modifier
{
    /// <summary>
    /// 숨쉬기 등의 자연스러운 흔들림
    /// </summary>
    [AddComponentMenu("PEACEMAKER/Animation/Modifiers/Wave Animation Modifier"), RequireComponent(typeof(ProceduralAnimation))]
    public class WaveAnimationModifier : ProceduralAnimationModifier
    {
        /// <summary>
        /// wave 속도 배율
        /// </summary>
        public float speedMultiplier = 1;
        
        /// <summary>
        /// 파동의 세기
        /// </summary>
        public float scaleMultiplier = 1;
        
        /// <summary>
        /// 위치에 적용할 파동 프로필
        /// </summary>
        public WaveProfile positionProfile = new ();
        
        /// <summary>
        /// 회전에 적용할 파동 프로필
        /// </summary>
        public WaveProfile rotationProfile = new ();
        
        /// <summary>
        /// 절차적 애니메이션과 동기화 하는지 여부
        /// </summary>
        public bool syncWithAnimation;
        
        /// <summary>
        /// 동기화 해제 시 멈추는 속도
        /// </summary>
        public float syncSpeed = 5;

        /// <summary>
        /// 동기화 시 곱해는 크기 배율
        /// </summary>
        private float _inScale = 1;

        private void Start()
        {
            // 파동을 업데이트 할 수 있는 상태로 만듦.
            positionProfile.Resume();
            rotationProfile.Resume();
        }

        private void Update()
        {
            // 1.현재 결과를 목표에 반영
            TargetPosition = positionProfile.result;
            TargetRotation = rotationProfile.result;

            // 2. 일시 정지면 업데이트 안함
            if (TargetAnimation.IsPaused) return;

            // 3.ProceduralAnimation의 진행도와 동기화 하는지 여부
            if (syncWithAnimation)
            {
                // 3-1.진행도를 파동 크기에 반영
                _inScale = TargetAnimation.Progress;

                // 3-2. 재생중이면 파동도 진행 가능 상태로
                if (TargetAnimation.IsPlaying)
                {
                    positionProfile.Resume();
                    rotationProfile.Resume();
                }
                // 3-3. 재생중이 아니면 파동을 일시 정지 상태로
                else
                {
                    positionProfile.Pause(syncSpeed);
                    rotationProfile.Pause(syncSpeed);
                }
            }
            // 4.동기화 안하는 경우
            else
            {
                // 배수를 1로 초기화
                _inScale = 1;
            }

            // 5. 파동 업데이트
            positionProfile.Update(speedMultiplier * GlobalSpeed, scaleMultiplier * _inScale);
            rotationProfile.Update(speedMultiplier * GlobalSpeed, scaleMultiplier * _inScale);
        }

        
        /// <summary>
        /// 위치 또는 회전에 대한 파동 계산을 담당.
        /// </summary>
        [Serializable]
        public class WaveProfile
        {
            /// <summary>
            /// 변화 시킬 시간
            /// </summary>
            public float smoothTime = 0;

            /// <summary>
            /// 각 축별 파동의 진폭
            /// </summary>
            [Space]
            public Vector3 amp;
            /// <summary>
            /// 각 축별 파동 속도
            /// </summary>
            public Vector3 speed = new Vector3(1, 1, 1);

            /// <summary>
            /// 현재 프레임에서 계산된 최종 파동 결과
            /// </summary>
            [HideInInspector]
            public Vector3 result;
            /// <summary>
            /// 내부 누적 시간 값.
            /// </summary>
            private Vector3 _time;

            /// <summary>
            /// 현재 파동이 정지인지 여부
            /// </summary>
            private bool _isPaused = false;

            // 각 축별 내부 속도 버퍼.
            private float _xV;
            private float _yV;
            private float _zV;

            /*-------------------------------
                    매 프레임 호출될 함수.
             ------------------------------*/
            public void Update(float globalSpeed, float globalAmount)
            {
                // 0. 일시 정지 중이면 종료
                if (_isPaused == true) return;

                // 1. 각 축별로 시간 증가.
                _time.x += Time.deltaTime * speed.x * globalSpeed;
                _time.y += Time.deltaTime * speed.y * globalSpeed;
                _time.z += Time.deltaTime * speed.z * globalSpeed;


                // 2. 각 축별로 파동 계산 (Sin에 증가된 시간값을 넣어서 계산)후 부르러운 변화
                result.x = UnityEngine.Mathf.SmoothDamp(result.x, amp.x * speed.x * Mathf.Sin(_time.x) * globalAmount, ref _xV, smoothTime);
                result.y = UnityEngine.Mathf.SmoothDamp(result.y, amp.y * speed.y * Mathf.Sin(_time.y) * globalAmount, ref _yV, smoothTime);
                result.z = UnityEngine.Mathf.SmoothDamp(result.z, amp.z * speed.z * Mathf.Sin(_time.z) * globalAmount, ref _zV, smoothTime);
            }

            /*-------------------------------
                        파동 일시 정지
             ------------------------------*/
            public void Pause(float pauseSpeed)
            {
                // 현재 결과를 0으로 보간.
                result = Vector3.Lerp(result, Vector3.zero, Time.deltaTime * pauseSpeed);
                // 파동 위상 초기화.
                _time = Vector3.zero;

                _isPaused = true;
            }

            /*-------------------------------
                       파동 재개
            ------------------------------*/
            public void Resume()
            {
                _isPaused = false;
            }
        }
    }
}
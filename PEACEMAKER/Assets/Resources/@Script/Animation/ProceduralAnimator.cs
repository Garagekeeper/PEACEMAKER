using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Resources.Script.Utilities;
using static Resources.Script.Defines;

namespace Resources.Script.Animation
{
    [AddComponentMenu("PEACEMAKER/Animation/Procedural Animator"), DisallowMultipleComponent]
    public class ProceduralAnimator : MonoBehaviour
    {
        /// <summary>
        /// 실제 애니메이션들을 들고 있는 게임 오브젝트
        /// </summary>
        public GameObject animationsHolder;
        /// <summary>
        /// 프레임 레이트
        /// </summary>
        public int frameRate = 165;
        /// <summary>
        /// 전체적인 가중치
        /// </summary>
        [Range(0, 1)]
        public float weight = 1;
        /// <summary>
        /// 위치에 관한 가중치
        /// </summary>
        [Range(0, 1)]
        public float positionWeight = 1;
        /// <summary>
        /// 회전에 관한 가중치
        /// </summary>
        [Range(0, 1)]
        public float rotationWeight = 1;

        /// <summary>
        /// 실제 클립을 저장하는 리스트
        /// </summary>
        [HideInInspector]
        public List<ProceduralAnimation> clips = new ();
        
        private bool _activeState = true;
        public bool IsActive
        {
            get
            {
                RefreshClips();

                foreach (ProceduralAnimation anim in clips)
                    anim.IsActive = _activeState;

                return _activeState;
            }
            
            set
            {
                // 클립 최신화
                RefreshClips();
                
                // 애니메이터 상태 변경
                _activeState = value;

                // 클립들의 상태를 애니메이터의 상태로 덮어씌움
                foreach (ProceduralAnimation anim in clips)
                    anim.IsActive = value;

            }
        }

        /// <summary>
        /// final position result from all clips
        /// 모든 클립의 결과가 적용된 마지막 위치
        /// </summary>
        private Vector3 _targetPosition;

        public Vector3 TargetPosition
        {
            get
            {
                Vector3 result = Vector3.zero;
                // 너무 짧은 시간은 예외처리
                if (Time.timeScale <= 0)
                    return result;

                // 재생할 클립이 없으면 바로 종료
                if (clips.Count <= 0) return result;

                // 모든 애니메이션 순회
                foreach (ProceduralAnimation clip in clips)
                {
                    if (!IsVector3Valid(clip.TargetPosition))
                    {
                        // 클립의 최종 벡터가 유효하지 않으면 오류
                        Debug.LogError($"{clip.procAnimName} outputs Nan position. Result animation position will be ignored.", clip);
                    }
                    else if (!clip.isolate)
                    {
                        // 클립이 독립적으로 움직이지 않으면
                        // 결과에 합산
                        result += clip.TargetPosition;
                    }
                }

                // 활성화 상태이면 결과까지 보간 
                if (_activeState)
                    _targetPosition = Vector3.Lerp(Vector3.zero, result, weight * positionWeight);

                return _targetPosition;
            }
            set => _targetPosition = value;
        }

        /// <summary>
        /// final rotation result from all clips
        /// 모든 클립의 결과가 적용된 마지막 회전
        /// </summary>
        private Vector3 _targetRotation;

        public Vector3 TargetRotation
        {
            get
            {
                Vector3 result = Vector3.zero;
                // 너무 짧은 시간은 예외처리
                if(Time.timeScale <= 0)
                    return result;
                
                // 재생할 클립이 없으면 바로 종료
                if (clips.Count <= 0) return result;
                
                // 모든 애니메이션 순회
                foreach (ProceduralAnimation clip in clips)
                {
                    if(!IsVector3Valid(clip.TargetPosition))
                    {
                        // 클립의 최종 벡터가 유효하지 않으면 오류
                        Debug.LogError($"{clip.procAnimName} outputs Nan rotation. Result animation rotation will be ignored.", clip);
                    }
                    else if(!clip.isolate)
                    {
                        // 클립이 독립적으로 움직이지 않으면
                        // 결과에 합산
                        result += clip.TargetRotation;
                    }
                }

                // 활성화 상태이면 결과까지 보간 
                if (_activeState)
                    _targetRotation = Vector3.Lerp(Vector3.zero, result, weight * rotationWeight);

                return _targetRotation;
            }
            set => _targetRotation = value;
        }

       

        /// <summary>
        /// 기준이 될 처음 위치
        /// </summary>
        private Vector3 DefaultPosition { get; set; }
        
        /// <summary>
        /// 기준이 될 처음 회전ㅣ
        /// </summary>
        private Vector3 DefaultRotation {  get; set; }
        
        public bool IsDefaultingInPosition(Vector3 tolerance, bool x = true, bool y = true, bool z = true)
        {
            Vector3 positionDifference = TargetPosition - DefaultPosition;

            if (positionDifference.x > tolerance.x && x || positionDifference.y > tolerance.y && y || positionDifference.z > tolerance.z && z) 
                return false;

            return true;
        }

        /// <summary>
        /// 회전의 변화량이 일정 범위 이하인지 반환하는 함수.
        /// </summary>
        /// <param name="tolerance"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool IsDefaultingInRotation(float tolerance, bool x = true, bool y = true, bool z = true)
        {
            Vector3 rotationDifference = TargetRotation - DefaultRotation;

            if (!x) rotationDifference.x = 0;
            if (!y) rotationDifference.y = 0;
            if (!z) rotationDifference.z = 0;

            return rotationDifference.magnitude <= tolerance;
        }

        private void Awake()
        {
            // 기준 위치 설정
            DefaultPosition = transform.localPosition;
            DefaultRotation = transform.localEulerAngles;

            // 애니메이션 홀더는 에디터에서 연결
            // 없으면 이 스크립트가 붙은 게임 오브젝트로 설정
            if(animationsHolder == null)
            {
                animationsHolder = gameObject;

                Debug.LogError($"AnimationHolder on {this} is not set. Setting it automaticly to self.", gameObject);
            }

            // 클립 최신화(추가)
            RefreshClips();
        }

        private void OnEnable()
        {
            RefreshClips();
        }

        /// <summary>
        /// 경과시간을 저장하는 변수
        /// </summary>
        private float ElapsedTime { get; set; } = 0;

        /// <summary>
        /// 최대 프레임 레이트
        /// </summary>
        private int _maxFramerate;

        private void Update()
        {
            if (Time.timeScale <= 0) return;

            // 경과시간 갱신
            ElapsedTime += Time.deltaTime;

            // 최대 프레임 레이트 갱신
            _maxFramerate = Mathf.Clamp(frameRate, 0, MaxAnimationFramerate);

            // 프레임 제한
            // 경과시간이 한 프레임에 걸리는 시간보다 같거나 크면
            // UpdateSingleFrame 실행
            //예: _maxFramerate = 60 → 1f / 60 = 0.0167초
            //즉, 16.7ms마다 한 번씩 UpdateSingleFrame 실행
            if (ElapsedTime >= 1f / _maxFramerate)
            {
                ElapsedTime = 0f;

                UpdateSingleFrame();
            }
        }

        /// <summary>
        /// 실제 위치/회전을 적용하는 함수
        /// </summary>
        private void UpdateSingleFrame()
        {
            if (Time.timeScale <= 0) return;

            // 위치 값 결정
            Vector3 position = DefaultPosition + TargetPosition;
            Quaternion rotation = Quaternion.identity;


            // 위치벡터가 유효하면 위치 이동.
            if (IsVector3Valid(position))
                transform.localPosition = position;
            // 아니면 오류 출력
            else
                Debug.LogError($"Total outputs Nan position. Result animation position will be ignored.", this);

            // 기준 회전과 목표 회전의 값이 유효하면
            if(IsVector3Valid(DefaultRotation) && IsVector3Valid(TargetRotation))
            {
                // 회전값 기록
                rotation = Quaternion.Euler(DefaultRotation + TargetRotation);
            }
            else
            {
                // 아니면 에러
                Debug.LogError($"Total outputs Nan rotation. Result animation rotation will be ignored.", this);
            }

            // 최종 값 또한 유효하면
            if (IsVector3Valid(DefaultRotation + TargetRotation))
                // 기록했던 회전 값 적용
                transform.localRotation = rotation;
        }

        public void Play(string animName)
        {
            ProceduralAnimation proceduralAnimation = GetAnimation(animName);

            proceduralAnimation.Play();
        }

        public void Play(string animName, float fixedTime)
        {
            ProceduralAnimation proceduralAnimation = GetAnimation(animName);

            if (proceduralAnimation)
                proceduralAnimation.Play(fixedTime);
        }

        public void Pause(string animName)
        {
            ProceduralAnimation proceduralAnimation = GetAnimation(animName);

            if (proceduralAnimation)
                proceduralAnimation.Pause();
        }

        public void Stop(string animName)
        {
            ProceduralAnimation proceduralAnimation = GetAnimation(animName);

            if (proceduralAnimation)
                proceduralAnimation.Stop();
        }

        /// <summary>
        /// returns all the animations clip for this animator in a List of ProceduralAnimationClip and refreshes the animtor clips 
        /// </summary>
        public List<ProceduralAnimation> RefreshClips()
        {
            clips = animationsHolder.GetComponentsInChildren<ProceduralAnimation>().ToList();

            return clips.ToList();
        }

        public ProceduralAnimation GetAnimation(string animName)
        {
            RefreshClips();

            ProceduralAnimation find = clips.Find(clip => clip.procAnimName == animName);

            return find;
        }
    }
}
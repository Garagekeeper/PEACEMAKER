using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static Resource.Script.Defines;
using Manager = Resource.Script.Managers.Managers;
using KickAnimationModifier = Resource.Script.Animation.Modifier.KickAnimationModifier;
using MoveAnimationModifier = Resource.Script.Animation.Modifier.MoveAnimationModifier;
using OffsetAnimationModifier = Resource.Script.Animation.Modifier.OffsetAnimationModifier;
using SpringAnimationModifier = Resource.Script.Animation.Modifier.SpringAnimationModifier;
using SwayAnimationModifier = Resource.Script.Animation.Modifier.SwayAnimationModifier;
using WaveAnimationModifier = Resource.Script.Animation.Modifier.WaveAnimationModifier;

namespace Resource.Script.Animation
{
    public class SeparatorAttribute : PropertyAttribute
    {
        public float thickness = 1f;
        public float padding = 6f;
    }
    /// <summary>
    /// 절차적 애니메이션을 정의하는 클래스.
    /// 이 클래스는 '하나의 애니메이션 동작(예: 반동, 흔들림, 스웨이 등)'을 담당하며,
    /// 여러 ProceduralAnimationModifier를 조합하여 결과적인 position/rotation을 계산한다.
    /// 
    /// ProceduralAnimator가 이 클래스들을 여러 개 모아서 최종 transform을 제어한다.
    /// </summary>
    [AddComponentMenu("PEACEMAKER/Animation/Procedural Animation")]
    public class ProceduralAnimation : MonoBehaviour
    {
        // ▣ 애니메이션 동작 타입
        public enum AnimationType
        {
            Override,   // 기본 위치/회전을 덮어쓰기
            Additive    // 기존 위치/회전에 더하기
        }

        // ────────────────────────────────────────────────────────────────
        // 기본 설정 영역
        // ────────────────────────────────────────────────────────────────
        /// <summary>
        /// 애니메이션 이름
        /// </summary>
        [Header("BASE"), Space]
        public string procAnimName = "New Procedural Animation"; 
        
        /// <summary>
        /// // 애니메이션 전체 길이(초 단위)
        /// </summary>
        public float length = 0.15f;
        
        /// <summary>
        /// // 이 애니메이션의 가중치 (blend용)
        /// </summary>
        [Range(0, 1)] public float weight = 1;
        
        /// <summary>
        /// 루프 여부
        /// </summary>
        public bool loop = false;
        
        /// <summary>
        /// progress 1 도달 시 자동 정지
        /// </summary>
        public bool autoStop = false;
        
        /// <summary>
        /// 연결(Connections)을 modifier 단위로 적용할지 여부
        /// </summary>
        public bool perModifierConnections = true;
        
        /// <summary>
        /// 오브젝트 활성 시 자동 재생 여부
        /// </summary>
        public bool playOnAwake = false;
        
        /// <summary>
        /// 한 번만 재생 가능하도록 제한
        /// </summary>
        public bool unidirectionalPlay = false;
        
        /// <summary>
        /// 재생 시 progress를 0으로 리셋
        /// </summary>
        public bool resetOnPlayed = false;
        
        /// <summary>
        /// 독립적으로 transform에 직접 반영할지 여부
        /// </summary>
        public bool isolate = false;
        
        /// <summary>
        /// isolate 시 동작 모드
        /// </summary>
        public AnimationType isolationMode = AnimationType.Override;
        
        /// <summary>
        /// Update, FixedUpdate, LateUpdate 중 어느 루프에서 Tick할지 설정
        /// </summary>
        public Defines.UpdateMode updateMode;

        // ────────────────────────────────────────────────────────────────
        // 입력 트리거 관련
        // ────────────────────────────────────────────────────────────────
        /// <summary>
        /// 트리거 동작 방식
        /// </summary>
        [Space]
        public InputActionType triggerType;
        /// <summary>
        /// Unity Input System의 입력 액션
        /// </summary>
        public InputActionReference inputActionReference;
        private InputAction _triggerInputAction = new();

        [Header("ASDF"), Space]
        // ────────────────────────────────────────────────────────────────
        // 이벤트 / 연결 / Modifier 목록
        // ────────────────────────────────────────────────────────────────
        /// <summary>
        /// OnPlay, OnStop 등 기본 이벤트
        /// </summary>
        [Space(6), Separator, Space(6)]
        public ProceduralAnimationEvents events = new ();
        
        ///// <summary>
        ///// 커스텀 이벤트 목록
        ///// </summary>
        //public List<CustomProceduralAnimationEvent> customEvents = new List<CustomProceduralAnimationEvent>(); 
        
        /// <summary>
        /// 다른 애니메이션과의 관계 설정
        /// </summary>
        public List<ProceduralAnimationConnection> connections = new ();    

        // 각각의 Modifier 타입별 캐싱
        public MoveAnimationModifier[] MoveAnimationModifiers { get; protected set; }
        public SpringAnimationModifier[] SpringAnimationModifiers { get; protected set; }
        public KickAnimationModifier[] KickAnimationModifiers { get; protected set; }
        public SwayAnimationModifier[] SwayAnimationModifiers { get; protected set; }
        public WaveAnimationModifier[] WaveAnimationModifiers { get; protected set; }
        public OffsetAnimationModifier[] OffsetAnimationModifiers { get; protected set; }

        public bool IsActive { get; set; } = true; // 전체 활성화 여부

        // ────────────────────────────────────────────────────────────────
        // 최종 결과값 (ProceduralAnimator가 참조)
        // ────────────────────────────────────────────────────────────────

        /// <summary>
        /// 최종 위치 결과
        /// Procedural Animator 에서 처리
        /// 이 프로퍼티를 호출하면 감쇠를 위한 함수 호출(GetTargetModifiersPosition)후 더하고
        /// 가중치를 더한뒤의 값을 반환
        /// </summary>
        public Vector3 TargetPosition => GetTargetModifiersPosition() * (weight * GlobalAnimationWeight);

        /// <summary>
        /// 최종 회전 결과
        /// Procedural Animator 에서 처리
        /// 이 프로퍼티를 호출하면 감쇠를 위한 함수 호출(GetTargetModifiersPosition)후 더하고
        /// 가중치를 더한뒤의 값을 반환
        /// </summary>
        public Vector3 TargetRotation => GetTargetModifiersRotation() * (weight * GlobalAnimationWeight);

        /// <summary>
        /// 현재 애니메이션 진행률 (0~1)
        /// </summary>
        public float Progress { get; set; }

        // 내부 상태
        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                // 트리거 타입을 None으로 바꿔서 수동 제어 전환
                triggerType = InputActionType.None;

                // 실행 방지 조건 확인
                _isPlaying = !HasToAvoid() && value;
            }
        }

        /// <summary>
        /// 일시 정지 여부
        /// </summary>
        public bool IsPaused { get; private set; } // 일시 정지 여부

        /// <summary>
        ///  Tab, DoubleTab등 상태 토글용
        /// </summary>
        private bool _isTriggered;
        
        /// <summary>
        ///  Tab, DoubleTab등 상태 토글용, 마지막 트리거 시간 기록
        /// </summary>
        private float _lastTriggerTime;
        
        /// <summary>
        /// progress 변화 속도 (SmoothDamp용)
        /// </summary>
        private float _currentVelocity;
        
        /// <summary>
        /// 현재 속도
        /// </summary>
        public float Velocity => _currentVelocity;
        

        /// <summary>
        /// 이 애니메이션에 포함된 모든 Modifier 
        /// </summary>
        public List<ProceduralAnimationModifier> Modifiers { get; set; } = new ();
        
        /// <summary>
        /// 항상 IDLE 상태로 유지할 것인지.
        /// </summary>
        public bool AlwaysStayIdle { get; set; } 

        /// <summary>
        /// 기본 로컬 위치
        /// </summary>
        private Vector3 _defaultPosition;
        
        /// <summary>
        /// 기본 로컬 회전
        /// </summary>
        private Quaternion _defaultRotation;

        
        // ────────────────────────────────────────────────────────────────
        // Unity 생명주기
        // ────────────────────────────────────────────────────────────────
        private void Awake()
        {
            _triggerInputAction = inputActionReference?.action;
            
            // 0. 자식에서 모든 Modifier 수집
            RefreshModifiers();
            // 1. 입력 액션 활성화
            _triggerInputAction?.Enable();

            // 2. 각 Modifier에 이 애니메이션을 참조로 연결
            foreach (ProceduralAnimationModifier modifier in Modifiers)
                modifier.TargetAnimation = this;

            // 타입별 Modifier 캐싱
            MoveAnimationModifiers = GetComponents<MoveAnimationModifier>();
            SpringAnimationModifiers = GetComponents<SpringAnimationModifier>();
            KickAnimationModifiers = GetComponents<KickAnimationModifier>();
            SwayAnimationModifiers = GetComponents<SwayAnimationModifier>();
            WaveAnimationModifiers = GetComponents<WaveAnimationModifier>();
            OffsetAnimationModifiers = GetComponents<OffsetAnimationModifier>();

            _defaultPosition = transform.localPosition;
            _defaultRotation = transform.localRotation;
        }

        private void OnEnable()
        {
            if (playOnAwake)
                Play(0);
        }

        private void Start()
        {
            // 부모 ProceduralAnimator에게 자기 자신을 등록
            GetComponentInParent<ProceduralAnimator>()?.RefreshClips();
        }

        // ────────────────────────────────────────────────────────────────
        // 업데이트 루프
        // ────────────────────────────────────────────────────────────────
        private void Update()
        {
            if (updateMode == Defines.UpdateMode.Update)
                Tick();

            if (gameObject.name == "Lean Right")
            {
                //Debug.Log(Progress);
                //Debug.Log(_triggerInputAction.IsPressed());
            }
        }

        private void FixedUpdate()
        {
            if (updateMode == Defines.UpdateMode.FixedUpdate)
                Tick();
        }

        private void LateUpdate()
        {
            if (updateMode == Defines.UpdateMode.LateUpdate)
                Tick();
        }

        // ────────────────────────────────────────────────────────────────
        // 메인 업데이트 루틴
        // ────────────────────────────────────────────────────────────────
        private Vector3 _isolatedPosition;
        private Quaternion _isolatedRotation;

        public void Tick()
        {
            if (!IsActive) return;

            // 커스텀 이벤트 및 재생 상태 변경 감지
            HandleEvents();

            // 연결된 애니메이션들과 상호작용 (예: PauseInIdle 등)
            // 지금 이 애니메이션과 연결된 연결의 목록
            // 연결은 target과 type 형태로 구성
            // 이 애니메이션이 target과 연결됨(type 형태로)
            foreach (ProceduralAnimationConnection connection in connections)
            {
                // target이 Idle상태(재생X)일때 정지하는 애니메이션이면
                if (connection.type == ProceduralAnimationConnectionType.PauseIfTargetIdle)
                {
                    // traget이 정지중이면 나도 정지
                    if (!connection.target.IsPlaying) Pause();
                    else Unpause();
                }

                // target이 input을 받아서 
                if (connection.type == ProceduralAnimationConnectionType.PauseIfTargetPlaying)
                {
                    // 타겟이 재생중이면 정지
                    // 재생중(Trigger)
                    if (connection.target.IsPlaying) Pause();
                    else Unpause();
                }
            }

            // ───────── 입력 액션(사용자 입력) 처리 ─────────
            // 누르고 있는 상태
            if (triggerType == InputActionType.Hold)
            {
                // 버튼을 누르고 있을 때 재생, 놓으면 정지
                // 이거는 나중에 숨참기 같은거 구현할때
                // TODO 숨참기  구현
                //if (_triggerInputAction.IsPressed() && Progress < 0.9f) Play();
                if (_triggerInputAction.IsPressed()) Play();
                else Stop();
            }

            // 한 번 탭
            if (triggerType == InputActionType.Tab)
            {
                // 한 번 누를 때마다 On/Off 토글
                if (_triggerInputAction.triggered) _isTriggered = !_isTriggered;

                if (_isTriggered) Play();
                else Stop();
            }

            // 두 번 탭
            if (triggerType == InputActionType.DoubleTab)
            {
                // 더블 탭 감지
                _triggerInputAction.HasDoubleClicked(ref _isTriggered, ref _lastTriggerTime, 0.3f);

                if (_isTriggered) Play();
                else Stop();
            }

            // 트리거
            if (triggerType == InputActionType.Trigger)
            {
                // 클릭 순간만 실행 (예: 반동)
                if (_triggerInputAction.triggered)
                    Play(0);
            }

            // ───────── 진행도 업데이트 ─────────
            if (!IsPaused)
                UpdateProgress();

            if (loop && Progress >= 0.999f)
                Progress = 0;

            if ((autoStop && Progress >= 0.999f) || HasToAvoid())
                Stop();

            // ───────── isolate 모드 처리 ─────────
            if (isolate)
            {
                _isolatedPosition = TargetPosition;
                _isolatedRotation = Quaternion.Euler(TargetRotation);

                if (isolationMode == AnimationType.Additive)
                {
                    // Additive 모드 → 현재 위치에 더함
                    _isolatedPosition += transform.localPosition;
                    _isolatedRotation *= transform.localRotation;
                }
                else
                {
                    // Override 모드 → 기본 위치 기준
                    _isolatedPosition += _defaultPosition;
                    _isolatedRotation *= _defaultRotation;
                }

                // 실제 Transform 적용
                transform.localPosition = TargetPosition + _defaultPosition;
                transform.localRotation = _isolatedRotation;
            }
        }

        // ────────────────────────────────────────────────────────────────
        // 애니메이션 제어 메서드
        // ────────────────────────────────────────────────────────────────
        public void Play(float fixedTime = -1)
        {
            // 단방향 재생일 경우 이미 실행 중이면 무시
            if (unidirectionalPlay && Progress > 0.1f)
                return;

            // 연결 유효성 검사
            foreach (var connection in connections)
            {
                if (!connection.target)
                    Debug.LogError($"[Procedural Animation] Connection's target reference is null or missing on {gameObject.name}.", gameObject);
            }

            // 중단해야되는 상황이 아닌 경우
            if (!HasToAvoid())
            {
                IsPaused = false;
                _isPlaying = true;
            }

            // 시작 시점 초기화
            if (resetOnPlayed)
                Progress = 0;
            else if (fixedTime >= 0)
                Progress = fixedTime;
            
            // 실행 가능 조건과 상관없이 입력이 들어오면 무조건 수행해야하는 이벤트 (총기 반동 등)
            events.OnPlay?.Invoke();
        }

        public void Pause() => IsPaused = true;
        public void Unpause() => IsPaused = false;
        public void Stop() => _isPlaying = false;

        // ────────────────────────────────────────────────────────────────
        // 진행도 갱신
        // ────────────────────────────────────────────────────────────────
        private void UpdateProgress()
        {
            //TODO 하드코딩 바꾸기
            float masterSpeed = 1;

            if (_isPlaying)
                Progress = Mathf.SmoothDamp(Progress, 1, ref _currentVelocity, length / masterSpeed);

            if (!_isPlaying || HasToAvoid())
                Progress = Mathf.SmoothDamp(Progress, 0, ref _currentVelocity, length / masterSpeed);
        }

        // ────────────────────────────────────────────────────────────────
        // 이벤트 처리
        // ────────────────────────────────────────────────────────────────
        private bool _prevPlaying;

        private void HandleEvents()
        {
            // 재생 상태 변경 감지
            // 실재 재생할 수 있는 조건의 애니메이션만 재생
            // 달리는 중에 조준 불가 처럼 조건을 따져야하는 경유
            if (_isPlaying && !_prevPlaying)
                events.OnPlayed?.Invoke();

            if (!_isPlaying && _prevPlaying)
                events.OnStoped?.Invoke();

            // 커스텀 이벤트 업데이트
            //foreach (var animationEvent in customEvents)
            //    animationEvent.UpdateEvent(this);

            _prevPlaying = _isPlaying;
        }

        // ────────────────────────────────────────────────────────────────
        // 유틸리티 함수들
        // ────────────────────────────────────────────────────────────────
        /// <summary>
        /// 사용될 모든 모디파이어를 List로 가져와 Modifires에 저장
        /// </summary>
        public void RefreshModifiers()
        {
            // 자식 오브젝트의 모든 Modifier 수집
            Modifiers = GetComponentsInChildren<ProceduralAnimationModifier>().ToList();
        }

        /// <summary>
        /// 연결된 애니메이션 상태에 따라 자기 자신을 중단해야 하는지 판단.
        /// </summary>
        public bool HasToAvoid()
        {
            if (AlwaysStayIdle || Manager.Game.ProcAnimIsActive == false)
                return true;

            var result = false;

            foreach (var connection in connections.Where(connection => connection.target))
            {
                // 특정 애니메이션이 실행 중이면 회피
                if (connection.type == ProceduralAnimationConnectionType.InfluenceIfTargetPlaying && connection.target._isPlaying)
                    result = true;

                // 특정 애니메이션이 Idle이면 회피
                if (connection.type == ProceduralAnimationConnectionType.InfluenceIfTargetIdle && !connection.target._isPlaying)
                    result = true;
            }

            return result;
        }

        public float GetInfluenceFactor(ProceduralAnimation procAnimation)
        {
            return procAnimation == null ? 0f :
                // 다른 애니메이션의 진행도에 따라 1→0으로 보간 (중첩 방지)
                Mathf.Lerp(1, 0, procAnimation.Progress);
        }

        /// <summary>
        /// Modifier들의 최종 Position 결과를 계산
        /// </summary>
        public Vector3 GetTargetModifiersPosition()
        {
            Vector3 result = Vector3.zero;
            float avoidanceFactor = 1;

            // 연결된 다른 애니메이션의 진행도에 따라 영향 줄이기
            foreach (var connection in connections)
            {
                if (connection.target == null) continue;

                switch (connection.type)
                {
                    // 현재 애니메이션이 연결된 애니메이션이 재생될 때 영향을 받는 설정이면
                    // (연결 대상이 재생중일 때 변화를 줘야한다면)
                    // 감쇠  배율을 구한다. 단 target의 progress를 기준으로 하기 때문에
                    // 정지중이면 함수에서 1을 반환해서 감쇠배수가 1 즉, 변화가 없음
                    case ProceduralAnimationConnectionType.InfluenceIfTargetPlaying:
                        avoidanceFactor *= GetInfluenceFactor(connection.target);
                        break;
                    // 현재 애니메이션이 연결된 애니메이션이 Idle일 때 영향을 받는 설정이면
                    // (연결 대상이 Idle일 때 변화를 줘야한다면)
                    // 감쇠  배율을 구한다. 단 target의 progress를 기준으로 하기 때문에
                    // 재생중이면 함수에서 0을 반환해서 감쇠배수가 1 즉, 변화가 없음
                    case ProceduralAnimationConnectionType.InfluenceIfTargetIdle:
                        avoidanceFactor *= 1-GetInfluenceFactor(connection.target);
                        break;
                }
            }

            // 모든 Modifier의 position 결과 합산
            foreach (var modifier in Modifiers)
                result += modifier.TargetPosition;

            if (perModifierConnections)
                result *= avoidanceFactor;

            return result;
        }

        /// <summary>
        /// Modifier들의 최종 Rotation 결과를 계산
        /// </summary>
        public Vector3 GetTargetModifiersRotation()
        {
            Vector3 result = Vector3.zero;
            float avoidanceFactor = 1;

            foreach (var connection in connections)
            {
                if (connection.target == null) continue;

                switch (connection.type)
                {
                    // 현재 애니메이션이 연결된 애니메이션이 재생될 때 영향을 받는 설정이면
                    // (연결 대상이 재생중일 때 변화를 줘야한다면)
                    // 감쇠  배율을 구한다. 단 target의 progress를 기준으로 하기 때문에
                    // 정지중이면 함수에서 1을 반환해서 감쇠배수가 1 즉, 변화가 없음
                    case ProceduralAnimationConnectionType.InfluenceIfTargetPlaying:
                        avoidanceFactor *= GetInfluenceFactor(connection.target);
                        break;
                    // 현재 애니메이션이 연결된 애니메이션이 Idle일 때 영향을 받는 설정이면
                    // (연결 대상이 Idle일 때 변화를 줘야한다면)
                    // 감쇠  배율을 구한다. 단 target의 progress를 기준으로 하기 때문에
                    // 재생중이면 함수에서 0을 반환해서 감쇠배수가 1 즉, 변화가 없음
                    case ProceduralAnimationConnectionType.InfluenceIfTargetIdle:
                        avoidanceFactor *= 1-GetInfluenceFactor(connection.target);
                        break;
                }
            }

            // Modifier 회전 합산
            foreach (var modifier in Modifiers)
                result += modifier.TargetRotation;

            if (perModifierConnections)
                result *= avoidanceFactor;

            return result;
        }

        // ────────────────────────────────────────────────────────────────
        // 입력 트리거 타입 정의
        // ────────────────────────────────────────────────────────────────
        public enum InputActionType
        {
            None = 0,
            Tab = 1,            // 토글
            Hold = 2,           // 누르는 동안만
            DoubleTab = 3,      // 더블탭
            Trigger = 4,        // 클릭 순간만
            HoldWithLimit = 5,  // 누르는 동안만 인데, 제한 시간이 존재 (숨참기 등) 
        }
    }
}

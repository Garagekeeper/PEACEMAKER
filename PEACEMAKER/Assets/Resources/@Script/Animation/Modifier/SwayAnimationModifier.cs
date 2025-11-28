using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resources.Script.Animation.Modifier
{
    /// <summary>
    /// 입력에 따른 반응형 흔들림
    /// FPS 무기 카메리의 SWAY를 구현하는 모디파이어
    /// 마우스를 움직이면 총기가 기우는등의 느낌
    /// </summary>
    public class SwayAnimationModifier : ProceduralAnimationModifier
    {
        public InputActionReference inputActionReference;
        private InputAction _inputAction;
        
        /// <summary>
        /// 강도
        /// </summary>
        public float amplification = 1;
        /// <summary>
        /// 부드러움의 정도
        /// 높을수록 천천히
        /// </summary>
        public float positionSmoothness = 5;
        /// <summary>
        /// 부드러움의 정도
        /// 높을수록 천천히
        /// </summary>
        public float rotationSmoothness = 10;
        /// <summary>
        /// 일시정지 일 때 활성화 여부
        /// </summary>
        public bool disableOnPaused = true;

        // 좌우 움직임
        [Header("Input X")]
        public Vector3 positionInputX;
        public Vector3 rotationInputX;

        // 상하 움직임
        [Header("Input Y")]
        public Vector3 positionInputY;
        public Vector3 rotationInputY;

        // 최대 움직임
        [Header("Limits")]
        public Vector3 positionLimit = Vector3.one;
        public Vector3 rotationLimit = Vector3.one;

        // 내부 계산 결과
        private Vector3 _resultPosition;
        private Vector3 _resultRotation;

        // 내부에서 추적하는 X,Y 입력
        protected float inputX;
        protected float inputY;
        
        public float InputX { get => inputX; set { inputX = value; IsControlledLocally = false; } }
        public float InputY { get => inputY; set { inputY = value; IsControlledLocally = false; } }

        protected void Start()
        {
            
        }

        public bool IsControlledLocally { get; protected set; } = true;

       
        protected void Update()
        {
            // 0.0 / 0.0 값이 들어오는거 방지
            if(float.IsNaN(InputX)) InputX = 0;
            if (float.IsNaN(InputY)) InputY = 0;

            _resultPosition = Vector3.zero;
            _resultRotation = Vector3.zero;
            
            // 실제 입력값 읽어오기
            var input = SystemManager.Input.Move;
            // 프레임 보정
            if (IsControlledLocally)
            {
                inputX = 0.5f * input.x / Time.deltaTime;
                inputY = 0.5f * input.y / Time.deltaTime;
            }
            
            // 입력에 따른 위치, 회전 계산
            Vector3 resultPositionInputX = positionInputX * inputX;
            Vector3 resultPositionInputY = positionInputY * inputY;
            
            Vector3 resultRotationInputX = rotationInputX * inputX;
            Vector3 resultRotationInputY = rotationInputY * inputY;

            // 최종 결과
            _resultPosition += resultPositionInputX + resultPositionInputY;
            _resultRotation += resultRotationInputX + resultRotationInputY;

            // 일시 정지중이면 멈춤
            if (SystemManager.Game.IsPaused)
            {
                _resultPosition = Vector3.zero;
                _resultRotation = Vector3.zero;
            }

            // 움직임 범위 제한
            _resultPosition.x = Mathf.Clamp(_resultPosition.x, -positionLimit.x, positionLimit.x);
            _resultPosition.y = Mathf.Clamp(_resultPosition.y, -positionLimit.y, positionLimit.y);
            _resultPosition.z = Mathf.Clamp(_resultPosition.z, -positionLimit.z, positionLimit.z);

            _resultRotation.x = Mathf.Clamp(_resultRotation.x, -rotationLimit.x, rotationLimit.x);
            _resultRotation.y = Mathf.Clamp(_resultRotation.y, -rotationLimit.y, rotationLimit.y);
            _resultRotation.z = Mathf.Clamp(_resultRotation.z, -rotationLimit.z, rotationLimit.z);

            // 강도 적용
            _resultPosition *= amplification;
            _resultRotation *= amplification;

            // 실제 이동
            TargetPosition = Vector3.Lerp(TargetPosition, _resultPosition, Time.deltaTime * positionSmoothness * GlobalSpeed);
            TargetRotation = Vector3.Lerp(TargetRotation, _resultRotation, Time.deltaTime * rotationSmoothness * GlobalSpeed);
        }
    }
}
using Resource.Script.Managers;
using UnityEngine;
using UnityEngine.Events;
using static Resource.Script.Defines;
using static Resource.Script.Utilities;

namespace Resource.Script.Controller
{
    /// <summary>
    /// Player의 실제 이동과 관련한 클래스
    /// 이동 점프 웅크리기 둘러보기
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("How quickly the player accelerates to the target movement speed.")]
        public float acceleration = 0.1f;

        [Tooltip("Default walking speed.")]
        public float walkSpeed = 5;

        [Tooltip("Movement speed while crouching.")]
        public float crouchSpeed = 3;

        [Tooltip("Movement speed while sprinting.")]
        public float sprintSpeed = 10;

        [Tooltip("How high the player can jump.")]
        public float jumpHeight = 6;

        public float defaultHeight;

        [Tooltip("Player's height when crouched.")]
        public float crouchHeight = 1.5f;

        [Tooltip("Distance between footstep sounds (lower = more frequent).")]
        public float stepInterval = 7;

        [Space]
        [Tooltip("Strength of gravity applied to the player.")]
        public float gravity = 1;

        [Tooltip("Maximum speed the player can fall.")]
        public float maxFallSpeed = 350;

        [Header("Camera")] [Tooltip("Camera For FPS")]
        public GameObject camobj;
        public Transform camRootTransform;
        public Camera FirstPersonCamera  { get; set; }
        
        [Header("Events")]
        [Tooltip("Invoked when the character leaves the ground.")]
        public UnityEvent onJump = new UnityEvent();

        [Tooltip("Invoked when the character touches the ground.")]
        public UnityEvent onLand = new UnityEvent();
        
        private Vector3 _finalVelocity;
        public Vector2 LookAfterModifySensitivity { get; set; }
        public Vector2 FinalLook { get; set; }
        float _currentXRotation;
        float _currentYRotation;
        public Quaternion CameraRotation {get; private set;}
        public Quaternion PlayerRotation {get; private set;}

        public bool IsCrouching { get; private set; } = false;
        public float SpeedMultiplier { get; private set; } = 1;
        
        public bool PrevGroundInfo { get; private set; }
        
        
        /// <summary>
        ///  Controller for Player
        /// </summary>
        public CharacterController CharacterController { get; set; }
        
        void Awake()
        {
            LockCursor();
            
            _finalVelocity = Vector3.zero;
            LookAfterModifySensitivity = Vector3.zero;

            FirstPersonCamera = camobj.GetComponent<Camera>();

        }
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            CharacterController = GetComponent<CharacterController>();
            defaultHeight = CharacterController.height;
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            
            /*-------------------------
            *          Speed
            -------------------------*/
            //float speed = 0f;
            //if (SystemManager.Input.SprintPressed ) speed = IsCrouching ? crouchSpeed * SpeedMultiplier : sprintSpeed * SpeedMultiplier;
            
            /*-------------------------
            *          Look
            -------------------------*/
            Vector2 rawLookUnscaled = SystemManager.Input.Look;
            
            // 가로, 세로 감도를 곱하고 전체적인 감도를 곱해준다. (체감되도록 100을 곱한다)
            LookAfterModifySensitivity = new Vector2(
                rawLookUnscaled.x * XSensitivityMultiplier,
                rawLookUnscaled.y * YSensitivityMultiplier
            ) * (100 * SensitivityMultiplier);
            
            float finalSensitivity = 1f;
            float deviceSensitivity = 1f;
            
            // TODO 플레이하는 컨트롤러에 따라서 장치 감도 조절 및 하드코딩 수정 (FPSCONTROLLER)
            deviceSensitivity = 1 * 0.1f;
            
            float baseSensitivity = deviceSensitivity;
            
            // TODO isDynamicSensitivityEnabled관련해서 알아보기
            finalSensitivity = baseSensitivity;
            
            if (SystemManager.Game.IsPaused)
                finalSensitivity = 0;
            
            // TODO addedLookValue의 기능 구현
            Vector2 addedLookValue = Vector2.zero;
            // 최종계산된 Look을 반영
            Vector2 calculatedLook = addedLookValue + (LookAfterModifySensitivity * finalSensitivity);
            FinalLook = (calculatedLook / 200f) + addedLookValue;
            //addedLookValue = Vector2.zero;
            UpdateRotation();
            
            /*-------------------------
            *       Crouching
            -------------------------*/
            
            IsCrouching = SystemManager.Input.CrouchToggle;
            // 달리기나 점프중이면 웅크리기 해제
            if (SystemManager.Input.SprintPressed && IsCrouching)
                IsCrouching = false;
            ApplyCrouching();
            
            /*-------------------------
            *          Move
            ------------------------*/
            if (CharacterController.isGrounded)
            {
                // 캐릭터 컨트롤러는 최상위에 달려있어서 camroot와 회전이 다르다.
                // 그래서 w를 눌렀을때 camroot가 보는 방향으로 가기 위해서 보정을 해준다.
                Vector3 moveDir = (camRootTransform.forward * SystemManager.Input.Move.y +
                                   camRootTransform.right * SystemManager.Input.Move.x).normalized;

                float moveSpeedMultiplier = walkSpeed;
                
                if (SystemManager.Input.SprintPressed)
                    moveSpeedMultiplier = sprintSpeed;
                
                if (IsCrouching)
                    moveSpeedMultiplier = crouchSpeed;
                
                
                _finalVelocity.x = moveDir.x * moveSpeedMultiplier;
                _finalVelocity.z = moveDir.z * moveSpeedMultiplier;
                
                if (SystemManager.Input.JumpPressed)
                // 점프키가 눌리면 점프 적용
                {
                    onJump?.Invoke();
                    IsCrouching = false;
                    ApplyCrouching();
                    _finalVelocity.y += jumpHeight - _finalVelocity.y;
                    
                }
                else
                {
                    // 땅에 붙어있어도 작은 중력이 작용
                    _finalVelocity.y = Physics.gravity.y * 0.5f;
                    
                    if (CharacterController.isGrounded && !PrevGroundInfo)
                        onLand?.Invoke();
                }
            }
            else if (CharacterController.velocity.magnitude * 3.5 < maxFallSpeed)
            {
                // 공중에 있으면 중력이 작용
                _finalVelocity.y += Physics.gravity.y * gravity * Time.deltaTime;
            }
            
            PrevGroundInfo = CharacterController.isGrounded;
            
            // 보정된 좌표로 이동
            CharacterController.Move(_finalVelocity * Time.deltaTime);
            //TODO 경사면 계산 추가

        }

        private void Jump()
        {
            Debug.Log("Jump");
        }
        

        /// <summary>
        /// 실제 회전을 적용시키는 함수.
        /// </summary>
        private void UpdateRotation()
        {
            // Look delta값 누적
            _currentYRotation += FinalLook.x;
            _currentXRotation -= FinalLook.y;
            
            // x회전의 경우는 클램프로 제한
            // "마우스를 위로 계속 올렸더니 한 바퀴 돌아서 원래대로 돌아왔어요(or 화면이 뒤집혔어요)!" 방지 
            _currentXRotation = Mathf.Clamp(_currentXRotation, -90f, 90f);
        
            // 쓰레기 값 필터링
            if (float.IsNaN(_currentXRotation)) _currentXRotation = 0;
            if (float.IsNaN(_currentYRotation)) _currentYRotation = 0;
            
            CameraRotation = Quaternion.Slerp(CameraRotation, Quaternion.Euler(_currentXRotation, _currentYRotation,0), Time.deltaTime * 100);
            PlayerRotation = Quaternion.Slerp(PlayerRotation, Quaternion.Euler(0, _currentYRotation,0), Time.deltaTime * 100);
            
            camRootTransform.transform.SetRotation(CameraRotation);
        }

        private void ApplyCrouching()
        {
            // 플레이어가 웅크린 상태면 높이 조절
            float height;
            if (IsCrouching && SystemManager.Input.CrouchToggle)
            {
                height = Mathf.Lerp(CharacterController.height, crouchHeight, Time.deltaTime * 15);
            }
            else
            {
                SystemManager.Input.CrouchToggle = false;
                height = Mathf.Lerp(CharacterController.height, defaultHeight, Time.deltaTime * 15);
            }

            CharacterController.height = height;
            CharacterController.center = Vector3.up * (CharacterController.height * 0.5f);
            camRootTransform.position = transform.position + ((Vector3.up * (CharacterController.height - 1) + new Vector3(0, -0.05f, 0) + CharacterController.center));
        }
    }

}
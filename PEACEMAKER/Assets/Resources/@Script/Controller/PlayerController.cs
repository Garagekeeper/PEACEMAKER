using System.Collections.Generic;
using Resources.Script.Audio;
using Resources.Script.Creatures;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.Events;
using static Resources.Script.Defines;
using static Resources.Script.Utilities;
using Resources.Script.Inventory;
using UnityEngine.Serialization;


namespace Resources.Script.Controller
{
    /// <summary>
    /// Player의 실제 이동과 관련한 클래스
    /// 이동 점프 웅크리기 둘러보기
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] [Tooltip("How quickly the player accelerates to the target movement speed.")]
        public float acceleration = 0.1f;

        [Tooltip("Default walking speed.")] public float walkSpeed = 5;

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

        [Space] [Tooltip("Strength of gravity applied to the player.")]
        public float gravity = 1;

        [Tooltip("Maximum speed the player can fall.")]
        public float maxFallSpeed = 350;

        [FormerlySerializedAs("camobj")] [Header("Camera")] [Tooltip("Camera For FPS")]
        public GameObject camObj;

        public Transform camRootTransform;
        public Camera FirstPersonCamera { get; set; }
        public AudioPreset landSfx;

        [Header("Events")] [Tooltip("Invoked when the character leaves the ground.")]
        public UnityEvent onJump = new UnityEvent();

        [Tooltip("Invoked when the character touches the ground.")]
        public UnityEvent onLand = new UnityEvent();

        private Vector3 _finalVelocity;
        public Vector2 LookAfterModifySensitivity { get; set; }
        public Vector2 FinalLook { get; set; }
        float _currentXRotation;
        float _currentYRotation;
        public Quaternion CameraRotation { get; private set; }
        public Quaternion PlayerRotation { get; private set; }

        public bool IsCrouching { get; private set; } = false;

        public bool PrevGroundInfo { get; private set; }
        private Vector2 _addedLookValue;
        public Transform PhysicalInventory { get; set; }
        
        protected IDamageable _ownerCombatStat;
        protected IMovable _ownerMoveStat;

        private void AddLookValue(Vector2 value)
        {
            if (!HeadManager.Game.IsPaused)
                _addedLookValue += value;
        }

        public void AddLookValue(float mouseX, float mouseY)
        {
            AddLookValue(new Vector2(mouseX, mouseY));
        }

        /// <summary>
        ///  Controller for Player
        /// </summary>
        public CharacterController CharacterController { get; private set; }

        public InventoryCore Inventory { get; set; }

        public List<FirearmController> initialFirearms;
        public List<string> SelectedFirearms { get; set; }

        void Awake()
        {
            _ownerCombatStat = GetComponent<IDamageable>();
            _ownerMoveStat = GetComponent<IMovable>();
            _finalVelocity = Vector3.zero;
            LookAfterModifySensitivity = Vector3.zero;
            _addedLookValue = Vector2.zero;
            FirstPersonCamera = camObj.GetComponent<Camera>();

            CharacterController = GetComponent<CharacterController>();
            defaultHeight = CharacterController.height;
            
            // Inventory 초기화
            Inventory = this.FindSelfChild<InventoryCore>();

            // 착지소리 등록
            //onLand.AddListener(() => HeadManager.Audio.PlayWithPreset(landSfx, transform));
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            LockCursor();

            // TODO 메인 화면에서 선택한 무기를 넣도록
            // 현재는 에디터에서 넣어주는 걸로만 하는 중
            if (initialFirearms.Count == 0)
            {
                Debug.LogError("no initial firearm founds");
            }
            else
            {
                foreach (var firearm in initialFirearms)
                {
                    var firearmInstance = Instantiate(firearm, Inventory.transform);
                    Inventory.AddItem2Inventory(firearmInstance);
                }

                Inventory.OffAll();
                Inventory.SwapItem(0);
            }
        }

        private void Update()
        {
            if (HeadManager.Game.IsPaused) return;
            UpdateInventory();
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if (HeadManager.Game.IsPaused) return;
            /*-------------------------
            *          Look
            -------------------------*/
            UpdateRotation();

            /*-------------------------
            *       Crouching
            -------------------------*/
            ApplyCrouching();

            /*-------------------------
            *          Move
            -------------------------*/
            UpdateMove();
        }


        private void UpdateMove()
        {
            if (CharacterController.isGrounded)
            {
                // 캐릭터 컨트롤러는 최상위에 달려있어서 camera root와 회전이 다르다.
                // 그래서 w를 눌렀을때 camera root가 보는 방향으로 가기 위해서 보정을 해준다.
                Vector3 moveDir = (camRootTransform.forward * HeadManager.Input.State.Move.y +
                                   camRootTransform.right * HeadManager.Input.State.Move.x).normalized;

                float moveSpeedMultiplier = walkSpeed * _ownerMoveStat.SpeedMultiplier;

                if (HeadManager.Input.State.SprintPressed)
                    moveSpeedMultiplier = sprintSpeed * _ownerMoveStat.SpeedMultiplier;


                if (Inventory.GetCurrentItem()?.FirearmState == EFirearmStates.Fire || HeadManager.Input.State.AimHeld)
                    moveSpeedMultiplier = walkSpeed * _ownerMoveStat.SpeedMultiplier;

                if (IsCrouching)
                    moveSpeedMultiplier = crouchSpeed * _ownerMoveStat.SpeedMultiplier;

                _finalVelocity.x = moveDir.x * moveSpeedMultiplier;
                _finalVelocity.z = moveDir.z * moveSpeedMultiplier;

                // 점프키가 눌리면 점프 적용
                if (HeadManager.Input.State.JumpPressed)
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
        
        private void UpdateInventory()
        {
            if (HeadManager.Input.State.InventoryPressed == null) return;
            Inventory.SwapItem((int)HeadManager.Input.State.InventoryPressed);
        }

        /// <summary>
        /// 실제 회전을 적용시키는 함수.
        /// </summary>
        private void UpdateRotation()
        {
            Vector2 rawLookUnscaled = HeadManager.Input.State.Look;

            // 가로, 세로 감도를 곱하고 전체적인 감도를 곱해준다. (체감되도록 100을 곱한다)
            LookAfterModifySensitivity = new Vector2(
                rawLookUnscaled.x * XSensitivityMultiplier,
                rawLookUnscaled.y * YSensitivityMultiplier
            ) * (100 * SensitivityMultiplier);

            float finalSensitivity = 1f;
            float deviceSensitivity = 1f;

            // TODO 플레이하는 컨트롤러에 따라서 장치 감도 조절 및 하드코딩 수정 (FPS CONTROLLER)
            deviceSensitivity = 1 * 0.1f;

            float baseSensitivity = HeadManager.Game.MouseSensitivity * deviceSensitivity;

            finalSensitivity = baseSensitivity;

            // TODO addedLookValue의 기능 구현

            // 최종계산된 Look을 반영
            Vector2 calculatedLook = _addedLookValue + (LookAfterModifySensitivity * finalSensitivity);
            FinalLook = (calculatedLook / 200f) + _addedLookValue;
            _addedLookValue = Vector2.zero;

            // Look delta값 누적
            _currentYRotation += FinalLook.x;
            _currentXRotation -= FinalLook.y;

            // x회전의 경우는 클램프로 제한
            // "마우스를 위로 계속 올렸더니 한 바퀴 돌아서 원래대로 돌아왔어요(or 화면이 뒤집혔어요)!" 방지 
            _currentXRotation = Mathf.Clamp(_currentXRotation, -90f, 90f);

            // 쓰레기 값 필터링
            if (float.IsNaN(_currentXRotation)) _currentXRotation = 0;
            if (float.IsNaN(_currentYRotation)) _currentYRotation = 0;

            CameraRotation = Quaternion.Slerp(CameraRotation, Quaternion.Euler(_currentXRotation, _currentYRotation, 0),
                Time.deltaTime * 100);
            PlayerRotation = Quaternion.Slerp(PlayerRotation, Quaternion.Euler(0, _currentYRotation, 0),
                Time.deltaTime * 100);

            camRootTransform.transform.SetRotation(CameraRotation);
        }

        private void ApplyCrouching()
        {
            IsCrouching = HeadManager.Input.State.CrouchState;
            // 달리기나 점프중이면 웅크리기 해제
            if (HeadManager.Input.State.SprintPressed && IsCrouching)
                IsCrouching = false;

            // 플레이어가 웅크린 상태면 높이 조절
            float height;
            if (IsCrouching && HeadManager.Input.State.CrouchState)
            {
                height = Mathf.Lerp(CharacterController.height, crouchHeight, Time.deltaTime * 15);
            }
            else
            {
                HeadManager.Input.State.CrouchState = false;
                height = Mathf.Lerp(CharacterController.height, defaultHeight, Time.deltaTime * 15);
            }

            CharacterController.height = height;
            CharacterController.center = Vector3.up * (CharacterController.height * 0.5f);
            camRootTransform.position = transform.position + ((Vector3.up * (CharacterController.height - 1) +
                                                               new Vector3(0, -0.05f, 0) + CharacterController.center));
        }
    }
}
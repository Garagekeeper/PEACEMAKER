using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// 여기서 값을 읽어서 인풋 상태를 저장.
namespace Resources.Script.Input
{
    public sealed class InputReader : MonoBehaviour
    {
        private PlayerInuptActions _playerInput;
        public PlayerInuptActions PlayerInputAction => _playerInput;
        
        public InputState State { get; private set; }
        private bool _isBound;


        private void Awake()
        {
            // 콜백 바인딩
            _playerInput = new PlayerInuptActions();
            BindCallBacks();
        }

        private void OnEnable()
        {
            _playerInput.Player.Enable();
        }

        private void Update()
        {
            if (State == null) return;

            // 움직임, 시야는 Update에서 처리(매순간 입력을 받아야)
            // 다른 놈들은 이벤트 형식으로 처리
            State.Move = _playerInput.Player.Move.ReadValue<Vector2>();
            State.Look = _playerInput.Player.Look.ReadValue<Vector2>();
        }

        private void LateUpdate()
        {
            if (State == null) return;
            // 프레임 끝에서 상태 초기화
            State.ClearFrame();
        }

        private void OnDisable()
        {
            _playerInput.Player.Disable();
        }

        private void OnDestroy()
        {
            UnBindCallbacks();
            _playerInput.Dispose();
        }

        public void Initialize(InputState state)
        {
            State = state;
        }

        private void BindCallBacks()
        {
            if (_isBound) return;

            // Fire
            _playerInput.Player.Fire.started += OnFireStarted;
            _playerInput.Player.Fire.performed += OnFirePerformed;
            _playerInput.Player.Fire.canceled += OnFireCanceled;

            // Aim
            _playerInput.Player.Aim.started += OnAimStarted;
            _playerInput.Player.Aim.performed += OnAimPerformed;
            _playerInput.Player.Aim.canceled += OnAimCanceled;

            // Reload
            _playerInput.Player.Reload.performed += OnReloadPerformed;
            _playerInput.Player.Reload.canceled += OnReloadCanceled;

            // Jump
            _playerInput.Player.Jump.performed += OnJumpPerformed;

            // Pause
            _playerInput.Player.Pause.performed += OnPausePerformed;

            // Sprint
            _playerInput.Player.Sprint.started += OnSprintStarted;
            _playerInput.Player.Sprint.performed += OnSprintPerformed;
            _playerInput.Player.Sprint.canceled += OnSprintCanceled;

            // Crouch toggle
            _playerInput.Player.Crouch.performed += OnCrouchPerformed;

            // Lean
            _playerInput.Player.LeanLeft.started += OnLeanLeftStarted;
            _playerInput.Player.LeanLeft.canceled += OnLeanLeftCanceled;

            _playerInput.Player.LeanRight.started += OnLeanRightStarted;
            _playerInput.Player.LeanRight.canceled += OnLeanRightCanceled;

            // Inventory
            _playerInput.Player.SelectInventory.performed += OnSelectInventoryPerformed;

            _isBound = true;
        }

        

        private void UnBindCallbacks()
        {
            if (!_isBound) return;

            _playerInput.Player.Fire.started -= OnFireStarted;
            _playerInput.Player.Fire.performed -= OnFirePerformed;
            _playerInput.Player.Fire.canceled -= OnFireCanceled;

            _playerInput.Player.Aim.started -= OnAimStarted;
            _playerInput.Player.Aim.performed -= OnAimPerformed;
            _playerInput.Player.Aim.canceled -= OnAimCanceled;

            _playerInput.Player.Reload.performed -= OnReloadPerformed;
            _playerInput.Player.Reload.canceled -= OnReloadCanceled;

            _playerInput.Player.Jump.performed -= OnJumpPerformed;

            _playerInput.Player.Pause.performed -= OnPausePerformed;

            _playerInput.Player.Sprint.started -= OnSprintStarted;
            _playerInput.Player.Sprint.performed -= OnSprintPerformed;
            _playerInput.Player.Sprint.canceled -= OnSprintCanceled;

            _playerInput.Player.Crouch.performed -= OnCrouchPerformed;

            _playerInput.Player.LeanLeft.started -= OnLeanLeftStarted;
            _playerInput.Player.LeanLeft.canceled -= OnLeanLeftCanceled;

            _playerInput.Player.LeanRight.started -= OnLeanRightStarted;
            _playerInput.Player.LeanRight.canceled -= OnLeanRightCanceled;

            _playerInput.Player.SelectInventory.performed -= OnSelectInventoryPerformed;

            _isBound = false;
        }
        

        #region Callbacks

        // -------------------------
        // |    Fire               |
        // -------------------------

        private void OnFireStarted(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.FireHeld = true;
        }
        
        private void OnFirePerformed(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.FirePressed = true;
        }
        
        private void OnFireCanceled(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.FireHeld = false;
            State.FirePressed = false;
        }
        
        
        // -------------------------
        // |    Aim                |
        // -------------------------

        private void OnAimStarted(InputAction.CallbackContext ctx)
        {
            if(State == null) return;
            State.AimHeld = true;
        }
        private void OnAimPerformed(InputAction.CallbackContext ctx)
        {
            if(State == null) return;
            State.AimPressed = true;
        }
        private void OnAimCanceled(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.AimHeld = false;
            State.AimPressed = false;
        }

        // -------------------------
        // |    Reload             |
        // -------------------------
        private void OnReloadPerformed(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.ReloadPressed = true;
        }

        private void OnReloadCanceled(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.ReloadReleased = true;
        }

        // -------------------------
        // |    Jump               |
        // -------------------------
        private void OnJumpPerformed(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.JumpPressed = true;
        }

        
        // -------------------------
        // |    Pause              |
        // -------------------------
        
        private void OnPausePerformed(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.PausePressed = true;
        }

        // -------------------------
        // |    Sprint             |
        // -------------------------
        
        private void OnSprintStarted(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.SprintHeld = true;
        }

        private void OnSprintPerformed(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.SprintPressed = true;
        }

        private void OnSprintCanceled(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.SprintHeld = false;
            State.SprintReleased = true;
        }

        
        // -------------------------
        // |    Crouch             |
        // -------------------------
        
        private void OnCrouchPerformed(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            // 토글 발생여부와 실제 적용할 상태를 변경.
            State.CrouchState = !State.CrouchState;
            State.CrouchToggled = true;
        }

        // -------------------------
        // |    Lean               |
        // -------------------------
        
        private void OnLeanLeftStarted(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.LeanLeftHeld = true;
        }

        private void OnLeanLeftCanceled(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.LeanLeftHeld = false;
        }

        private void OnLeanRightStarted(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.LeanRightHeld = true;
        }

        private void OnLeanRightCanceled(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            State.LeanRightHeld = false;
        }
        
        // -------------------------
        // |    Inventory slot     |
        // -------------------------
        
        private void OnSelectInventoryPerformed(InputAction.CallbackContext ctx)
        {
            if (State == null) return;
            
            // 키보드 입력이 아니면 종료
            if (ctx.control is not KeyControl key)
            {
                State.InventoryPressed = null;
                return;
            }

            // 키보드 입력이 1~9사이에서 동작
            if (key.keyCode is >= Key.Digit1 and <= Key.Digit9)
            {
                int slot = (int)key.keyCode - (int)Key.Digit1;
                State.InventoryPressed = slot;
            }
            else
            {
                State.InventoryPressed = null;
            }
        }

        #endregion

    }
}
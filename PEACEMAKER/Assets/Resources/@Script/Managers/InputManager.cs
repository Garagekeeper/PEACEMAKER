using UnityEngine;
using UnityEngine.InputSystem;
using static Resources.Script.Defines;
using static Resources.Script.Utilities;

namespace Resource.Script.Managers
{
    public class InputManager : IManagerBase
    {
        private PlayerInuptActions _playerInput;
        public PlayerInuptActions PlayerInput => _playerInput;
        
        public InputManager()
        {
            _playerInput = new PlayerInuptActions();
            _playerInput.Player.Enable();
            
            //Fire
            // 단발
            _playerInput.Player.Fire.performed += ctx => FirePressed = true;
            _playerInput.Player.Fire.canceled += ctx =>
            {
                FireReleased = true;
                FirePressed = false;
            };
            // 지속
            _playerInput.Player.Fire.started += ctx => FireHeld = true;
            _playerInput.Player.Fire.canceled += ctx => FireHeld = false;
            
            //Aim
            // 단발
            _playerInput.Player.Aim.performed += ctx => AimPressed = true;
            _playerInput.Player.Aim.canceled += ctx =>
            {
                AimReleased = true;
                AimPressed = false;
            };
            // 지속
            _playerInput.Player.Aim.started += ctx => AimHeld = true;
            _playerInput.Player.Aim.canceled += ctx => AimHeld = false;
            
            // Reload
            
            // Pause
            _playerInput.Player.Pause.performed += ctx => Menu = true;
            
            // Sprint
            _playerInput.Player.Sprint.performed += ctx => SprintPressed = true;
            _playerInput.Player.Sprint.canceled += ctx =>
            {
                SprintPressed = false;
                SprintReleased = true;
            };
            
            // Crouch
            _playerInput.Player.Crouch.performed += ctx => CrouchToggle = !CrouchToggle;
            
            //Lean
            _playerInput.Player.LeanLeft.started += ctx => LeanLeftInput = true;
            _playerInput.Player.LeanLeft.canceled += ctx => LeanLeftInput = false;
            _playerInput.Player.LeanRight.started += ctx => LeanRightInput = true;
            _playerInput.Player.LeanRight.canceled += ctx => LeanRightInput = false;
            
        }
        
        public Vector2 Move {get ; private set;}
        public Vector2 Look {get ; private set;}
        public bool FirePressed {get ; private set;}
        public bool FireReleased {get ; private set;}
        public bool FireHeld{get ; private set;}
        public bool ReloadPressed { get; private set; }
        public bool JumpPressed {get ; private set;}
        public bool Menu { get; private set; }   // Pause 토글 상태
        public bool SprintPressed {get ; private set;}
        public bool SprintReleased {get ; private set;}
        public bool SprintHeld {get ; private set;}
        public bool CrouchToggle {get; set;}
        
        public bool AimPressed {get; private set;}
        public bool AimReleased {get; private set;}
        public bool AimHeld {get; private set;}
        
        public bool LeanLeftInput { get; private set; }
        public bool LeanRightInput { get; private set; }
        
        
        public void OnUpdate()
        {
            Move = _playerInput.Player.Move.ReadValue<Vector2>();
            Look = _playerInput.Player.Look.ReadValue<Vector2>();
            JumpPressed = _playerInput.Player.Jump.triggered;
            
            // 마우스 클릭 시 커서 잠금 복귀
            if (!IsCursorLocked && Mouse.current.leftButton.wasPressedThisFrame)
            {
                LockCursor();
            }
            
        }

        public void LateUpdate()
        {
            // 1프레임 입력만 유지되어야 하는 변수들만 초기화합니다.
            FirePressed = false;
            ReloadPressed = false;
            FireReleased = false;
        }
        
        private void OnPausePressed()
        {
            Menu = !Menu; // 토글

            if (Menu)
                UnlockCursor();
            else
                LockCursor();
        }
    }
}

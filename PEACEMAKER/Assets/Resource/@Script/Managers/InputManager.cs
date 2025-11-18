using UnityEngine;
using UnityEngine.InputSystem;
using static Resource.Script.Defines;
using static Resource.Script.Utilities;

namespace Resource.Script.Managers
{
    public class InputManager : IManagerBase
    {
        private PlayerInuptActions _playerInput;
        
        public InputManager()
        {
            _playerInput = new PlayerInuptActions();
            _playerInput.Player.Enable();
            
            //Fire
            // 단발
            _playerInput.Player.Fire.performed += ctx => FirePressed = true;
            _playerInput.Player.Fire.canceled += ctx => FireReleased = true;
            // 지속
            _playerInput.Player.Fire.started += ctx => FireHeld = true;
            _playerInput.Player.Fire.canceled += ctx => FireHeld = false;
            
            // Reload
            
            // Pause
            _playerInput.Player.Pause.performed += ctx => Menu = true;
            
            // Sprint
            _playerInput.Player.Sprint.performed += ctx => SprintPressed = true;
            _playerInput.Player.Sprint.canceled += ctx => SprintPressed = false;
            
            // Crouch
            _playerInput.Player.Crouch.performed += ctx => CrouchToggle = !CrouchToggle;
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
        public bool CrouchToggle {get; set;}
        
        
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

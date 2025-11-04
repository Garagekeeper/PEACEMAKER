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
            _playerInput.Player.Fire.performed += ctx => FirePressed = true;
            _playerInput.Player.Fire.canceled += ctx => FirePressed = false;
            
            //Jump
            _playerInput.Player.Jump.performed += ctx => JumpPressed = true;
            _playerInput.Player.Jump.canceled += ctx => JumpPressed = false;
            
            // Pause
            _playerInput.Player.Pause.performed += ctx => Menu = true;
            
            _playerInput.Player.Sprint.performed += ctx => SprintPressed = true;
            _playerInput.Player.Sprint.performed += ctx => SprintPressed = false;
        }
        
        public Vector2 Move {get ; private set;}
        public Vector2 Look {get ; private set;}
        public bool FirePressed {get ; private set;}
        public bool JumpPressed {get ; private set;}
        public bool Menu { get; private set; }   // Pause 토글 상태
        public bool SprintPressed {get ; private set;}
        
        
        public void OnUpdate()
        {
            Move = _playerInput.Player.Move.ReadValue<Vector2>();
            Look = _playerInput.Player.Look.ReadValue<Vector2>();
            //JumpPressed = _playerInput.Player.Jump.ReadValue<bool>();
            //FirePressed = _playerInput.Player.Fire.ReadValue<bool>();
            
            // 마우스 클릭 시 커서 잠금 복귀
            if (!IsCursorLocked && Mouse.current.leftButton.wasPressedThisFrame)
            {
                LockCursor();
            }
            
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

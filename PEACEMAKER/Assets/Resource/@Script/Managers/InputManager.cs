using UnityEngine;
using UnityEngine.InputSystem;

namespace Resource.Script.Managers
{
    public class InputManager : IManagerBase
    {
        private PlayerInuptActions _playerInput;
        
        public InputManager()
        {
            _playerInput = new PlayerInuptActions();
            _playerInput.Player.Enable();
            
            _playerInput.Player.Fire.performed += ctx => FirePressed = true;
            _playerInput.Player.Fire.canceled += ctx => FirePressed = false;
            _playerInput.Player.Jump.performed += ctx => JumpPressed = true;
            _playerInput.Player.Jump.canceled += ctx => JumpPressed = false;
        }
        
        public Vector2 Move {get ; private set;}
        public bool JumpPressed {get ; private set;}
        public bool FirePressed {get ; private set;}
        
        public void OnUpdate()
        {
            Move = _playerInput.Player.Move.ReadValue<Vector2>();
            //JumpPressed = _playerInput.Player.Jump.ReadValue<bool>();
            //FirePressed = _playerInput.Player.Fire.ReadValue<bool>();
        }
    }
}

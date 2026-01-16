using System;
using Resources.Script.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static Resources.Script.Defines;
using static Resources.Script.Utilities;

namespace Resources.Script.Managers
{
    public class InputManager
    {
        public InputState State { get; } = new InputState();

        private readonly InputReader _reader;
        
        public InputReader Reader => _reader;

        public InputManager(InputReader reader)
        {
            _reader = reader;
            _reader.Initialize(State);
        }

        public void Enable()  => _reader.enabled = true;
        public void Disable() => _reader.enabled = false;
    }
}

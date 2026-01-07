using System;
using Resources.Script.Controller;
using Resources.Script.Creatures;
using UnityEngine;
using static Resources.Script.Utilities;

namespace Resources.Script.Managers
{
    public class GameManager : MonoBehaviour
    {
        public bool IsPaused { get; set; } = false;
        public bool ProcAnimIsActive {get; private set;} = false;

        public int CurrentFirearmNum { get; set; } = -1;

        public Player MainPlayer { get; set; }
        
        public float MouseSensitivity { get; set; } = 1f;

        private void Awake()
        {
            ProcAnimIsActive = true;
        }

        public void Reset()
        {
            SystemManager.Game.IsPaused = false;
            UnlockCursor();
            CurrentFirearmNum = -1;
            MainPlayer = null;
            MouseSensitivity = 1f;
        }
    }
}
using System;
using Resources.Script.Controller;
using Resources.Script.Creature;
using UnityEngine;

namespace Resources.Script.Managers
{
    public class GameManager : MonoBehaviour
    {
        public bool IsPaused { get; set; } = false;
        public bool ProcAnimIsActive {get; private set;} = false;

        public int CurrentFirearmNum { get; set; } = -1;

        public Player MainPlayer { get; set; }

        private void Awake()
        {
            ProcAnimIsActive = true;
        }
    }
}
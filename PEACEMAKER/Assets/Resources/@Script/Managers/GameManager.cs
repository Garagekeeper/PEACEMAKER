using System;
using Resources.Script.Controller;
using Resources.Script.Creature;
using UnityEngine;

namespace Resources.Script.Managers
{
    public class GameManager : MonoBehaviour
    {
        public bool IsPaused { get; private set; } = false;
        public bool ProcAnimIsActive {get; private set;} = true;

        public int CurrentFirearmNum { get; set; } = -1;

        public Player MainPlayer { get; set; }

        private void Awake()
        {
        }
    }
}
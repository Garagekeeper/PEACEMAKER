using System;
using Resources.Script.Creatures;
using Resources.Script.Scene;
using Resources.Script.UI;
using Resources.Script.UI.Scene;
using Resources.Script.UI.Scene.MainScene;
using Resources.Script.UI.Setting;
using UnityEngine;
using static Resources.Script.Utilities;
using static Resources.Script.Defines;

namespace Resources.Script.Managers
{
    public class GameManager
    {
        public bool IsPaused { get; set; } = false;
        public bool IsPauseMenuOn { get; set; } = false;
        public bool ProcAnimIsActive { get; private set; } = false;

        public int CurrentFirearmNum { get; set; } = -1;

        public Player MainPlayer { get; set; }

        public float MouseSensitivity { get; set; } = 1f;

        public Action OnPause { get; set; }
        public Action OnMainMenu { get; set; }
        public Action OnSettingMenu { get; set; }
        public Action<float> OnScoreChanged { get; set; }
        public Action OnPlayerDeath { get; set; }
        
        public bool IsPlayerDead { get; set; } = false;

        private int _pauseCnt = 0;
        public float Score {get; private set;}

        public void Init()
        {
            ProcAnimIsActive = true;
            IsPaused = false;
            UnlockCursor();
            CurrentFirearmNum = -1;
            MainPlayer = null;
            //MouseSensitivity = 1f;
        }

        public void HandleEsc()
        {
            if (HeadManager.UI.SceneUI is UIMainScene)
            {
                if (HeadManager.UI.PopupCount == 0) return;
                if (HeadManager.UI.PeekPopup() is SettingMenu)
                {
                    PopPopup();
                    UnlockCursor();
                    return;
                }
            }
            else if (HeadManager.UI.SceneUI is UIGameScene)
            {
                if (IsPlayerDead)  return;
                
                if (HeadManager.UI.PopupCount == 0)
                {
                    Pause();
                    return;
                }

                if (HeadManager.UI.PeekPopup() is not (PauseMenu or SettingMenu))
                {
                    Pause();
                    return;
                }

                if (HeadManager.UI.PeekPopup() is PauseMenu)
                {
                    PopPopup();
                    return;
                }

                if (HeadManager.UI.PeekPopup() is SettingMenu)
                {
                    PopPopup();
                    return;
                }
            }
        }

        public void Pause()
        {
            IsPaused = true;
            _pauseCnt++;
            UnlockCursor();
            Time.timeScale = 0;
            OnPause?.Invoke();
        }

        public void PopPopup()
        {
            if (HeadManager.UI.PopupCount == 1)
            {
                IsPaused = false;
                LockCursor();
                Time.timeScale = 1;
            }

            HeadManager.UI.PeekPopup().ClosePopup();
            HeadManager.UI.PopPopup();
        }

        public void OnSetting()
        {
            OnSettingMenu?.Invoke();
        }

        public void LoadMain()
        {
            HeadManager.Loading.LoadScene("Main Menu");
            HeadManager.UI.StackClear();
        }

        public void LoadGameScene()
        {
            HeadManager.Loading.LoadScene("GameScene");
            HeadManager.UI.StackClear();
        }

        public void AddScore(ERarity val)
        {
            var finalVal = ((int)val + 1) * 100;
            Score += finalVal;
            OnScoreChanged?.Invoke(Score);
        }
        
        /// <summary>
        /// 플레이어가 죽었을때 메인으로 돌아가기 버튼을 누르면 실행될 함수
        /// </summary>
        public void OnGotoMainScene()
        {
            Score = 0;
            LoadMain();
        }

        /// <summary>
        /// 플레이어가 죽으면 실행될 함수
        /// </summary>
        public void PlayerDeath()
        {
            OnPlayerDeath?.Invoke();
            IsPlayerDead = true;
            Time.timeScale = 0;
            UnlockCursor();
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
        }
    }
}
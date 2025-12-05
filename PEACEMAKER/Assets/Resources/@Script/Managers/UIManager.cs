using System;
using System.Collections.Generic;
using Resources.Script.Controller;
using Resources.Script.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Resources.Script.Managers
{
    public class UIManager : MonoBehaviour
    {
        private Canvas currentCanvas;
        public Dictionary<FirearmController, FirearmHUD> FirearmHUDs { get; set; } = new (4);
        public Crosshair Crosshair { get; set; }
        public GameObject UIRoot { get; private set; }
        //private PauseMenu currentPauseMenu;

        private void Awake()
        {
            if (UIRoot == null)
            {
                UIRoot = new GameObject("@UIRoot")
                {
                    layer = LayerMask.NameToLayer("UI")
                };

                // Canvas
                Canvas canvas = UIRoot.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                // CanvasScaler
                var scaler = UIRoot.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);

                // GraphicRaycaster
                UIRoot.AddComponent<UnityEngine.UI.GraphicRaycaster>();

                DontDestroyOnLoad(UIRoot);
            }
        }

        public void OnSceneLoaded()
        {
            Reconnect();
        }

        private void Reconnect()
        {
            // 씬에서 Canvas 검색
            currentCanvas = FindFirstObjectByType<Canvas>();

            if (currentCanvas == null)
            {
                Debug.Log("이 씬엔 UI가 없음");
                return;
            }
            else
            {
                Debug.Log(currentCanvas);
            }
            
        }

        public void AddNewFirearmHUD(FirearmController controller, FirearmHUD preset)
        {
            FirearmHUD temp;
            FirearmHUDs.Add(controller,temp = Instantiate(preset));
            temp.transform.SetParent(UIRoot.transform, false);
            temp.Firearm = controller;
            FirearmHUDs[controller].Firearm = controller;
        }
        
        public void AddCrossHair(FirearmController controller, Crosshair preset)
        {
            Crosshair = Instantiate(preset);
            Crosshair.transform.SetParent(UIRoot.transform, false);
            Crosshair.Firearm = controller;
        }

        public void TogglePause()
        {

        }
    }
}
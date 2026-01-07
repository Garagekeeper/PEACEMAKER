using System;
using System.Collections.Generic;
using Resources.Script.Controller;
using Resources.Script.Creatures;
using Resources.Script.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Resources.Script.Utilities;

namespace Resources.Script.Managers
{
    public class UIManager : MonoBehaviour
    {
        public Dictionary<FirearmController, FirearmHUD> FirearmHUDs { get; set; } = new(4);
        public Crosshair Crosshair { get; set; }
        public GameObject UIRoot { get; private set; }

        [field: SerializeField] public VisualizedHpEffect HpEffect { get; private set; }

        [field: SerializeField] public Hitmarker Hitmarker { get; private set; }

        [field: SerializeField] public PlayerCardHUD PlayerCardHUDRef { get; set; }
        public PlayerCardHUD PlayerCardHUDIns { get; set; }

        public GameObject HUDObject { get; set; }
        public GameObject MenuObject { get; set; }

        [field: SerializeField] public MenuController MenuController { get; set; }
        [field: SerializeField] public AbilityPanelController AbilityController { get; set; }

        //private PauseMenu currentPauseMenu;

        private void Awake()
        {
            if (UIRoot == null)
            {
                GameObject uiRoot = GameObject.Find("@UIRoot");
                if (uiRoot != null)
                {
                    GameObject uiRootIn = GameObject.Find("@UIRootInstance");
                    if (uiRootIn != null)
                    {
                        Destroy(uiRoot.gameObject);
                        return;
                    }

                    uiRoot.name = "@UIRootInstance";
                    UIRoot = uiRoot;
                    UIRoot.layer = LayerMask.NameToLayer("UI");
                    HUDObject = GameObject.Find("HUD");
                    MenuObject = GameObject.Find("Menu");
                    
                    if (HpEffect == null)
                        Debug.LogError("There is no HpEffect assigned!");
                
                    HpEffect = Instantiate(HpEffect);
                    HpEffect.transform.SetParent(HUDObject.transform, false);
                
                    if (Hitmarker == null)
                        Debug.LogError("There is no Hitmarker assigned!");
                
                    Hitmarker = Instantiate(Hitmarker);
                    Hitmarker.transform.SetParent(HUDObject.transform, false);
                    
                    HUDObject.SetActive(false);
                }
                else
                {
                    UIRoot = new GameObject("@UIRootInstance")
                    {
                        layer = LayerMask.NameToLayer("UI")
                    };

                    
                    MenuObject = new GameObject("@Menu");

                    // Canvas
                    Canvas canvas = MenuObject.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    // CanvasScaler
                    var scaler = MenuObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                    scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);

                    // GraphicRaycaster
                    MenuObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }

                Init();
            }
        }

        void Init()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName.Contains("Game"))
            {
                HUDObject.SetActive(true);
            }
            else if (sceneName.Contains("Main"))
            {
                DestroyHUDObject();
                SystemManager.UI.MenuController.OpenMainMenu();
                UnlockCursor();
                HUDObject.SetActive(false);
            }
        }
        

        void DestroyHUDObject()
        {
            foreach (var HUD in (FirearmHUDs))
            {
                Destroy(HUD.Value.gameObject);
            }
            
            FirearmHUDs.Clear();
            
            if (PlayerCardHUDIns != null)
                Destroy(PlayerCardHUDIns.gameObject);
            
            if (Crosshair != null)
                Destroy(Crosshair.gameObject);

        }

        public void OnSceneLoaded()
        {
            //Reconnect();
            Init();
        }

        private void Reconnect()
        {
            
        }

        public void AddNewFirearmHUD(FirearmController controller, FirearmHUD preset)
        {
            FirearmHUD temp;
            FirearmHUDs.Add(controller, temp = Instantiate(preset));
            temp.transform.SetParent(HUDObject.transform, false);
            temp.Firearm = controller;
            FirearmHUDs[controller].Firearm = controller;
        }

        public void AddCrossHair(FirearmController controller, Crosshair preset)
        {
            if (Crosshair == null)
            {
                Crosshair = Instantiate(preset);
                Crosshair.transform.SetParent(HUDObject.transform, false);
            }
            Crosshair.Firearm = controller;
        }

        public void AddPlayerCard(Player player)
        {
            PlayerCardHUDIns = Instantiate(PlayerCardHUDRef);
            PlayerCardHUDIns.transform.SetParent(HUDObject.transform, false);
            PlayerCardHUDIns.Setup(player);
            PlayerCardHUDIns.UpdateCard();
            PlayerCardHUDIns.Enable();
        }

        public void OnOffAbilityUI(bool wantOn)
        {
            AbilityController.gameObject.SetActive(wantOn);
        }

        public void TogglePause()
        {
        }
    }
}
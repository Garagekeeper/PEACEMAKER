using System;
using System.Collections.Generic;
using Resources.Script.Controller;
using Resources.Script.Creatures;
using Resources.Script.Inventory;
using Resources.Script.UI;
using Resources.Script.UI.Firearm;
using Resources.Script.UI.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Resources.Script.Utilities;
using FirearmHUD = Resources.Script.UI.Firearm.FirearmHUD;
using Object = UnityEngine.Object;

namespace Resources.Script.Managers
{
    public class UIManager
    {
        private int _order = 10;
        private Stack<UIPopup> _popupStack = new();

        public UIScene SceneUI { get; private set; } = null;

        public T ShowSceneUI<T>(string name = null) where T : UIScene
        {
            if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

            //TODO 임시
            var go = HeadManager.Resource.Instantiate(Defines.EObjectID.GameSceneUI);
            T sceneUI = go.GetComponent<T>();
            if (sceneUI is UIGameScene sceneView)
            {
                // 인벤토리 등 필요한 모델을 찾아서 주입
                var player = Object.FindFirstObjectByType<Player>(); 
                var presenter = new InGameScenePresenter(sceneView, player);
                presenter.Init(); // 이벤트 구독 시작
                
            }

            SceneUI = sceneUI;
            
            return sceneUI;
        }
        
        public T ShowUI<T>(string name = null) where T : UIBase
        {
            if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

            //TODO 임시
            var go = HeadManager.Resource.Instantiate(Defines.EObjectID.FirearmHUD);
            T view = go.GetComponent<T>();
            
            if (view is FirearmHUD hudView)
            {
                // 인벤토리 등 필요한 모델을 찾아서 주입
                var inventory = Object.FindFirstObjectByType<InventoryCore>(); 
                var presenter = new FirearmHUDPresenter(hudView, inventory);
                presenter.Init(); // 이벤트 구독 시작
            }
            
            return view;
        }
    }
}
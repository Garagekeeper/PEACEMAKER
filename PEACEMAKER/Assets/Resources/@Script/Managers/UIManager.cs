using System;
using System.Collections.Generic;
using Resources.Script.Controller;
using Resources.Script.Creatures;
using Resources.Script.Inventory;
using Resources.Script.UI;
using Resources.Script.UI.Firearm;
using Resources.Script.UI.Scene;
using Resources.Script.UI.Scene.MainScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Resources.Script.Utilities;
using FirearmHUD = Resources.Script.UI.Firearm.FirearmHUD;
using Object = UnityEngine.Object;

namespace Resources.Script.Managers
{
    public class UIManager
    {
        //private int _order = 10;
        private Stack<UIPopup> _popupStack = new();
        private InGameScenePresenter _IGpresenter;
        private UIMainScenePresenter _Mainpresenter;
        //private InGameScenePresenter _presenter;

        public UIScene SceneUI { get; private set; } = null;

        public T ShowSceneUI<T>(string name = null) where T : UIScene
        {
            if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

            //TODO 임시
            GameObject go = null;
            if (name.Contains("GameScene"))
                go = HeadManager.Resource.Instantiate(Defines.EObjectID.GameSceneUI);
            if (name.Contains("Main"))
                go = HeadManager.Resource.Instantiate(Defines.EObjectID.MainSceneUI);

            if (go == null)
            {
                Debug.LogError("there is no Scene ui");
            }
            
            T sceneUI = go.GetComponent<T>();
            if (sceneUI is UIGameScene uiGameScene)
            {
                if (_IGpresenter != null)
                {
                    _IGpresenter.Release();
                    _IGpresenter = null;
                }
                
                if (_Mainpresenter != null)
                {
                    _Mainpresenter.Release();
                    _Mainpresenter = null;
                }
                
                // 인벤토리 등 필요한 모델을 찾아서 주입
                var player = Object.FindFirstObjectByType<Player>(); 
                _IGpresenter = new InGameScenePresenter(uiGameScene, player);
                _IGpresenter.Init(); // 이벤트 구독 시작
            }

            if (sceneUI is UIMainScene uiMainScene)
            {
                if (_IGpresenter != null)
                {
                    _IGpresenter.Release();
                    _IGpresenter = null;
                }
                
                if (_Mainpresenter != null)
                {
                    _Mainpresenter.Release();
                    _Mainpresenter = null;
                }
                
                _Mainpresenter = new UIMainScenePresenter(uiMainScene, HeadManager.Game);
                _Mainpresenter.Init();
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

        public void OpenPopup(UIPopup  popup)
        {
            _popupStack.Push(popup);
        }

        public UIPopup PopPopup()
        {
            return _popupStack.Pop();
        }
        
        public UIPopup PeekPopup()
        {
            return _popupStack.Peek();
        }

        public void StackClear()
        {
            while (_popupStack.Count > 0)
                PopPopup();
        }
        
        public int PopupCount => _popupStack.Count;
    }
}
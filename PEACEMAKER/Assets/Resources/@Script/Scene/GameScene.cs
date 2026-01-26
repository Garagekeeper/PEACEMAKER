using System;
using Resources.Script.Creatures;
using Resources.Script.Managers;
using Resources.Script.UI.Firearm;
using Resources.Script.UI.Scene;
using UnityEngine;

namespace Resources.Script.Scene
{
    public class GameScene : BaseScene
    {
        private UIGameScene sceneUI;
        private Transform _target;
        private Transform _minimapCam;
        private Transform _mainCamRoot;
        private void Awake()
        {
            //TODO 임시
            HeadManager.Audio.ReSetting();
            Init();
            
        }

        public override void Init()
        {
            //Enemy enemy = HeadManager.ObjManager.Spawn<Enemy>(Defines.EObjectID.Enemy, new Vector3(0, -8, 0));
            var test = HeadManager.UI.ShowSceneUI<UIGameScene>();
            _target = HeadManager.Game.MainPlayer.transform;
            _minimapCam = GetComponentInChildren<Camera>().transform;
            _mainCamRoot = _target.Find("CameraRoot").transform;
        }

        private void LateUpdate()
        {
            _minimapCam.transform.position = _target.position + Vector3.up*5;
            _minimapCam.transform.rotation = Quaternion.Euler(90f, _mainCamRoot.rotation.eulerAngles.y, 0f);
        }
    }
}
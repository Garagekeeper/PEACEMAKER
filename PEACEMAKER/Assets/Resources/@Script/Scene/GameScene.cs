using Resources.Script.Controller;
using Resources.Script.Creatures;
using Resources.Script.Managers;
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

        public override void Init()
        {
            HeadManager.UI.ShowSceneUI<UIGameScene>();
            base.Init();
            _target = HeadManager.Game.MainPlayer.transform;
            _minimapCam = GetComponentInChildren<Camera>().transform;
            _mainCamRoot = _target.Find("CameraRoot").transform;
            HeadManager.Game.IsPlayerDead = false;
        }
        

        private void LateUpdate()
        {
            _minimapCam.transform.position = _target.position + Vector3.up*5;
            _minimapCam.transform.rotation = Quaternion.Euler(90f, _mainCamRoot.rotation.eulerAngles.y, 0f);
        }
    }
}
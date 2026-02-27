using System;
using Resources.Script.Controller;
using Resources.Script.Creatures;
using Resources.Script.Managers;
using Resources.Script.UI.Scene;
using Unity.VisualScripting;
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
            base.Init();
            HeadManager.Game.IsPlayerDead = false;
            PreWarmPool();
            HeadManager.UI.ShowSceneUI<UIGameScene>();
        }

        private void PreWarmPool()
        {
            var soDict = HeadManager.Resource.SourceCatalog.SoDict;
            Transform rootTransform = null;
            foreach (var soPair in soDict)
            {
                switch (soPair.Key)
                {
                    case Defines.EObjectID.Enemy:
                        rootTransform = HeadManager.ObjManager.EnemiesRoot;
                        break;
                    case Defines.EObjectID.ExpGemNormal:
                    case Defines.EObjectID.ExpGemRare:
                    case Defines.EObjectID.ExpGemEpic:
                        rootTransform = HeadManager.ObjManager.ExpGemsRoot;
                        break;
                    case Defines.EObjectID.SFX:
                        rootTransform = HeadManager.ObjManager.SoundRoot;
                        break;
                    case Defines.EObjectID.DMGFX:
                        rootTransform = HeadManager.ObjManager.DmgRoot;
                        break;
                }
                
                HeadManager.Pool.CreatePoolExternal(soPair.Value,  rootTransform);
            }
        }

        private void Start()
        {
            
            _target = HeadManager.Game.MainPlayer.transform;
            _mainCamRoot = _target.Find("CameraRoot").transform;
            _minimapCam = GetComponentInChildren<Camera>().transform;
        }


        private void LateUpdate()
        {
            _minimapCam.transform.position = _target.position + Vector3.up*5;
            _minimapCam.transform.rotation = Quaternion.Euler(90f, _mainCamRoot.rotation.eulerAngles.y, 0f);
        }
    }
}
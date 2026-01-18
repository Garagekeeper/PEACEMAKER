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
        }
    }
}
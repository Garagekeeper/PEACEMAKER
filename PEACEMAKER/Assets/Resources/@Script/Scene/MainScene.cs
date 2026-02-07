using Resources.Script.Managers;
using Resources.Script.UI.Scene.MainScene;

namespace Resources.Script.Scene
{
    public class MainScene : BaseScene
    {
        public override void Init()
        {
            base.Init();
            HeadManager.UI.ShowSceneUI<UIMainScene>();
            
            // INIT AFTER SCENE LOAD
            HeadManager.ObjManager.Init();
            HeadManager.Ability.Init();
        }
    }
}
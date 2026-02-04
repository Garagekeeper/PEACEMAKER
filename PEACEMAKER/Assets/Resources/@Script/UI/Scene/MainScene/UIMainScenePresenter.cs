using Resources.Script.Managers;
using Resources.Script.UI.Setting;

namespace Resources.Script.UI.Scene.MainScene
{
    public class UIMainScenePresenter : Presenter<UIMainScene, GameManager>
    {
        private MainMenuPresenter _mainMenuPresenter;
        private SettingMenuPresenter _settingMenuPresenter;
        
        public UIMainScenePresenter(UIMainScene view, GameManager model) : base(view, model)
        {
        }

        public override void Init()
        {
            _mainMenuPresenter = new MainMenuPresenter(view.MainMenu,  model);
            _settingMenuPresenter = new SettingMenuPresenter(view.SettingsMenu, HeadManager.Game);
            
            _mainMenuPresenter.Init();
            _settingMenuPresenter.Init();
        }

        public override void Release()
        {
            _mainMenuPresenter.Release();
            _settingMenuPresenter.Release();
        }
    }
}
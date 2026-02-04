using Resources.Script.Managers;

namespace Resources.Script.UI
{
    public class MainMenuPresenter : Presenter<MainMenu, GameManager>
    {
        public MainMenuPresenter(MainMenu view, GameManager model) : base(view, model)
        {
        }

        public override void Init()
        {
            view.StartClicked += OnStartClicked;
            view.SettingsClicked += OnSettingsClicked;
            view.ExitClicked += OnExitClicked;
        }

        public void OnStartClicked()
        {
            model.LoadGameScene();
        }

        public void OnSettingsClicked()
        {
            model.OnSetting();
        }
        
        public void OnExitClicked()
        {
            model.ExitGame();
        }

        public override void Release()
        {
            view.StartClicked -= OnStartClicked;
            view.SettingsClicked -= OnSettingsClicked;
            view.ExitClicked -= OnExitClicked;
        }
    }
}
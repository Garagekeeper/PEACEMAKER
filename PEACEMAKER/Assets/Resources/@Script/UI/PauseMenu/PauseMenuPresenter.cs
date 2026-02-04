using Resources.Script.Managers;

namespace Resources.Script.UI
{
    public class PauseMenuPresenter : Presenter<PauseMenu, GameManager>
    {
        public PauseMenuPresenter(PauseMenu view, GameManager model) : base(view, model)
        {
        }

        public override void Init()
        {
            view.ResumeClicked += OnResume;
            view.MainMenuClicked += model.LoadMain;
            view.SettingsClicked += model.OnSetting;
            model.OnPause += OnPause;
        }

        public void OnPause()
        {
            HeadManager.UI.OpenPopup(view);
            view.Show();
        }

        public void OnResume()
        {
            if (view == HeadManager.UI.PeekPopup())
            {
                view.Hide();
                model.PopPopup();
            }
        }

        public override void Release()
        {
            view.ResumeClicked -= model.PopPopup;
            view.MainMenuClicked -= model.LoadMain;
            view.SettingsClicked -= model.OnSetting;
            model.OnPause -= OnPause;
        }
    }
}
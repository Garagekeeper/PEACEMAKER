using Resources.Script.Managers;

namespace Resources.Script.UI.Death
{
    public class DeathUIPresenter : Presenter<Death.DeathUI, GameManager>
    {
        public DeathUIPresenter(DeathUI view, GameManager model) : base(view, model)
        {
        }

        public override void Init()
        {
            view.onGotoMain += GotoMainScene;
            model.OnPlayerDeath += ShowView;
        }

        public void ShowView()
        {
            view.Show();
        }

        public void GotoMainScene()
        {
            view.Hide();
            model.OnGotoMainScene();
        }

        public override void Release()
        {
            view.onGotoMain -= GotoMainScene;
            model.OnPlayerDeath -= ShowView;
        }
    }
}
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Resources.Script.UI
{
    public class PauseMenu : Menu
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public void OnSettingsButton()
        {
            SystemManager.UI.MenuController.OpenMenu("Setting Menu");
        }

        public void OnResumeButton()
        {
            SystemManager.UI.MenuController.PopMenu();
        }
        
        public void OnGotoMainMenuButton()
        {
            SystemManager.Game.Reset();
            SystemManager.UI.MenuController.PopMenu();
            SceneManager.LoadSceneAsync("Main Menu");
        }
    }
}
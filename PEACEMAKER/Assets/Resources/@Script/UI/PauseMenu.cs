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
            HeadManager.UI.MenuController.OpenMenu("Setting Menu");
        }

        public void OnResumeButton()
        {
            HeadManager.UI.MenuController.PopMenu();
        }
        
        public void OnGotoMainMenuButton()
        {
            HeadManager.Game.Init();
            HeadManager.UI.MenuController.PopMenu();
            //SystemManager.Loading.LoadScene("Main Menu");
            SceneManager.LoadSceneAsync("Main Menu");
        }
    }
}
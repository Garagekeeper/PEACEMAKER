using UnityEngine;

namespace Resources.Script.UI
{
    public class MainMenu : Menu
    {
        public void OnStartButton()
        {
            // 게임 시작 로직
        }

        public void OnSettingsButton()
        {
            controller.OpenMenu("SettingsMenu");
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }
    }
}
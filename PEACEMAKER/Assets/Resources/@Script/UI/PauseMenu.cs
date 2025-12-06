using UnityEngine;

namespace Resources.Script.UI
{
    public class PauseMenu : Menu
    {
        protected override void Awake()
        {
            base.Awake();
            OnClose();
        }

        public void OnSettingsButton()
        {
            controller.OpenMenu("SettingsMenu");
        }

        public void OnResumeButton()
        {
            controller.PopMenu();
        }
    }
}
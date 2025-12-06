using UnityEngine;

namespace Resources.Script.UI
{
    public class SettingsMenu : Menu
    {
        public void OnBackButton()
        {
            controller.PopMenu(); // 자동 Back
        }
    }
}
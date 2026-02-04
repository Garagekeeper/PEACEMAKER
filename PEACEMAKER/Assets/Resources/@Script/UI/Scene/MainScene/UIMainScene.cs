using Resources.Script.Managers;
using Resources.Script.UI.Firearm;
using Resources.Script.UI.Setting;
using UnityEngine;

namespace Resources.Script.UI.Scene.MainScene
{
    public class UIMainScene : UIScene, IView
    {
        [SerializeField] private MainMenu mainMenu = null;
        [SerializeField] private SettingMenu settingMenu = null;
        
        public MainMenu MainMenu => mainMenu;
        public SettingMenu SettingsMenu => settingMenu;
        
        private void Update()
        {
            if (HeadManager.Input.State.PauseState)
                HandleEsc();
        }

        public void HandleEsc()
        {
            HeadManager.Game.HandleEsc();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
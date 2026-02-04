using System.Collections.Generic;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Resources.Script.UI.Setting
{
    public class SettingMenuPresenter : Presenter<SettingMenu, GameManager>
    {
        SettingManager _settingManager;
        public SettingMenuPresenter(SettingMenu view, GameManager model) : base(view, model)
        {
        }

        public override void Init()
        {
            //이 2개 순서를 반드시 지켜
            UpdateResolution();
            view.LoadUIValues(HeadManager.Setting.Data);
            
            view.onApply += OnApply;
            view.onQuit += OnQuit;
            model.OnSettingMenu += OpenSettingMenu;
        }

        private void UpdateResolution()
        {
            var options = new List<string>();

            foreach (var res in Screen.resolutions)
            {
                options.Add($"{res.width} x {res.height} ({res.refreshRateRatio}Hz)");
            }
            
            view.UpdateResolution(options);
        }

        public void OpenSettingMenu()
        {
            view.Show();
            HeadManager.UI.OpenPopup(view);
        }
        
        public void OnApply(SettingData data)
        {
            HeadManager.Setting.SetMasterVolume(data.masterVolume);
            HeadManager.Setting.SetMouseSensitivity(data.mouseSensitivity);
            HeadManager.Setting.SetResolution(data.resolutionIndex);
            HeadManager.Setting.SetFullscreen(data.fullscreen);
            HeadManager.Setting.ApplySettings();
        }

        public void OnQuit()
        {
            HeadManager.UI.PopPopup();
            view.Hide();
        }

        public override void Release()
        {
            view.onApply -= OnApply;
            view.onQuit -= OnQuit;
            model.OnSettingMenu -= OpenSettingMenu;
        }
    }
}
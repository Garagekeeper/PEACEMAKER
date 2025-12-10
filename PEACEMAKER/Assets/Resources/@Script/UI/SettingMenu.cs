using Akila.FPSFramework;
using Resources.Script.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Script.UI
{
    public class SettingsMenu : Menu
    {
        [Header("UI References")]
        public Slider volumeSlider;
        public Slider sensitivitySlider;
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown fullscreenDropdown;
        
        private float _volumeVal;
        private float  _mouseSensitivityVal;
        private int _resIndexVal;
        private int _fullscreenVal;
        private int _screenType;
        

        private bool initialized = false;

        private void Start()
        {
            InitResolutionDropdown();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            InitResolutionDropdown();
            LoadUIValues();
        }
        

        private void InitResolutionDropdown()
        {
            //이건 테스트용
            if (initialized) return;
            if (resolutionDropdown == null) return;
            

            resolutionDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();

            foreach (var res in Screen.resolutions)
            {
                options.Add($"{res.width} x {res.height} ({res.refreshRateRatio}Hz)");
            }

            resolutionDropdown.AddOptions(options);
            initialized = true;
        }

        private void LoadUIValues()
        {
            if (!initialized) return;

            var data = SystemManager.Setting.Data;

            _volumeVal = data.masterVolume * 100f;
            _resIndexVal = data.resolutionIndex;
            _mouseSensitivityVal = data.mouseSensitivity;
            _screenType = data.fullscreen;
            volumeSlider.SetValueWithoutNotify(_volumeVal);
            sensitivitySlider.SetValueWithoutNotify(_mouseSensitivityVal);
            resolutionDropdown.SetValueWithoutNotify(_resIndexVal);
            fullscreenDropdown.SetValueWithoutNotify(_screenType);
        }

        // =========================
        //      UI Listeners
        // =========================
        public void OnVolumeChanged(float value)
        {
            _volumeVal = value;
        }

        public void OnSensitivityChanged(float value)
        {
            _mouseSensitivityVal = value;
        }

        public void OnResolutionChanged(int index)
        {
            _resIndexVal = index;
        }

        public void OnFullscreenChanged(int index)
        {
            _screenType = index;
        }

        public void OnApplyButton()
        {
            SystemManager.Setting.SetMasterVolume(_volumeVal);
            SystemManager.Setting.SetMouseSensitivity(_mouseSensitivityVal);
            SystemManager.Setting.SetResolution(_resIndexVal);
            SystemManager.Setting.SetFullscreen(_screenType);
            SystemManager.Setting.ApplySettings();
            SystemManager.UI.MenuController.PopMenu();
        }
        
        public void OnQuitButton()
        {
            SystemManager.UI.MenuController.PopMenu();
        }
    }
}
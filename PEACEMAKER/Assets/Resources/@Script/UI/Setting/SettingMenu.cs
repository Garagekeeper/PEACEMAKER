using System;
using System.Collections.Generic;
using Resources.Script.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Script.UI.Setting
{
    public class SettingMenu : UIPopup, IView
    {
        [Header("UI References")]
        public Slider volumeSlider;
        public Slider sensitivitySlider;
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown fullscreenDropdown;
        
        [SerializeField] private ButtonController applyBtn;
        [SerializeField] private ButtonController quitBtn;
        
        private float _volumeVal;
        private float  _mouseSensitivityVal;
        private int _resIndexVal;
        private int _fullscreenVal;
        private int _screenType;

        public SettingData Data { get; set; }
        
        public event Action<SettingData> onApply;
        public event Action onQuit;
        

        private bool initialized = false;


        private void Awake()
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            fullscreenDropdown.onValueChanged.AddListener(OnFullscreenChanged);

            applyBtn.onClick += OnApplyButton;
            quitBtn.onClick += OnQuitButton;
        }

        public void UpdateResolution(List<string> options)
        {
            //이건 테스트용
            if (initialized) return;
            if (resolutionDropdown == null) return;
            
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);
            initialized = true;
        }

        public void LoadUIValues(SettingData data)
        {
            if (!initialized) return;

            Data = data;
            
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
            Data.masterVolume = value / 100f;
        }

        public void OnSensitivityChanged(float value)
        {
            _mouseSensitivityVal = value;
            Data.mouseSensitivity = value;
        }

        public void OnResolutionChanged(int index)
        {
            _resIndexVal = index;
            Data.resolutionIndex = index;
        }

        public void OnFullscreenChanged(int index)
        {
            _screenType = index;
            Data.fullscreen = index;
        }

        public void OnApplyButton()
        {
            onApply?.Invoke(Data);
            
            //HeadManager.UI.MenuController.PopMenu();
        }
        
        public void OnQuitButton()
        {
            onQuit?.Invoke();
            //HeadManager.UI.MenuController.PopMenu();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void ClosePopup()
        {
            Hide();
        }
    }
}
using System.IO;
using Resources.Script.UI;
using UnityEngine;

namespace Resources.Script.Managers
{
    public class SettingManager
    {
        public SettingData Data { get; private set; }

        private string filePath;

        public void Init()
        {
            filePath = Path.Combine(Application.persistentDataPath, "settings.json");

            LoadSettings();
            //ApplySettings();
        }

        // ----------------------------
        //       SETTINGS APPLY
        // ----------------------------
        public void ApplySettings()
        {
            SaveSettings();
            LoadSettings();
        }

        // ----------------------------
        //       UPDATE VALUES
        // ----------------------------
        public void SetMasterVolume(float v)
        {
            Data.masterVolume = v;
            AudioListener.volume = v;
        }

        public void SetMouseSensitivity(float v)
        {
            Data.mouseSensitivity = v;
        }

        public void SetResolution(int index)
        {
            if (Data.resolutionIndex != index) Data.resolutionIndex = index;

            //Resolution
            var resolutions = Screen.resolutions;
            if (Data.resolutionIndex >= 0 && Data.resolutionIndex < resolutions.Length)
            {
                var res = resolutions[Data.resolutionIndex];
                // 0: 전체, 1:보더리스, 2: 창모드
                Screen.SetResolution(res.width, res.height, (FullScreenMode)Data.fullscreen);
            }
        }

        public void SetFullscreen(int value)
        {
            if (Data.fullscreen != value)
            {
                Data.fullscreen = value;
            }
        }

        // ----------------------------
        //       SAVE / LOAD JSON
        // ----------------------------
        public void SaveSettings()
        {
            string json = JsonUtility.ToJson(Data, true);
            File.WriteAllText(filePath, json);

            Debug.Log($"Settings saved to: {filePath}");
        }

        public void LoadSettings()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                Data = JsonUtility.FromJson<SettingData>(json);
                SetMasterVolume(Data.masterVolume);
                SetMouseSensitivity(Data.mouseSensitivity);
                SetFullscreen(Data.fullscreen);
                SetResolution(Data.resolutionIndex);
            }
            else
            {
                // 처음 실행 시 기본값 생성
                Data = new SettingData();
                SaveSettings();
            }
        }
    }
}
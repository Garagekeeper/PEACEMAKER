using System.IO;
using Resources.Script.UI;
using UnityEngine;

namespace Resources.Script.Managers
{
    public class SettingManager : MonoBehaviour
    {
        public SettingData Data { get; private set; }

        private string filePath;

        private void Awake()
        {
            filePath = Path.Combine(Application.persistentDataPath, "settings.json");

            LoadSettings();
            ApplySettings();
        }

        // ----------------------------
        //       SETTINGS APPLY
        // ----------------------------
        public void ApplySettings()
        {
            SaveSettings();
        }

        // ----------------------------
        //       UPDATE VALUES
        // ----------------------------
        public void SetMasterVolume(float v)
        {
            Data.masterVolume = v / 100;
            AudioListener.volume = v / 100f;
        }

        public void SetMouseSensitivity(float v)
        {
            Data.mouseSensitivity = v;
        }

        public void SetResolution(int index)
        {
            if (Data.resolutionIndex == index) return;
            Data.resolutionIndex = index;

            //Resolution
            var resolutions = Screen.resolutions;
            if (Data.resolutionIndex >= 0 && Data.resolutionIndex < resolutions.Length)
            {
                var res = resolutions[Data.resolutionIndex];
                // 0: 전체, 1:보더리스, 2: 창모드
                Screen.SetResolution(res.width, res.height, Data.fullscreen != 2);
            }
        }

        public void SetFullscreen(int value)
        {
            if (Data.fullscreen == value) return;
            Data.fullscreen = value;
            // 0: 전체, 1:보더리스, 2: 창모드
            Screen.fullScreen = value != 2;
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
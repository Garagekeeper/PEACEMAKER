using Akila.FPSFramework;
using Resources.Script.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Resources.Script.UI
{
    public class MainMenu : Menu
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1;
        }

        public void LoadGame(string name)
        {
            //LoadingScreen.LoadScene(name);
            SceneManager.LoadSceneAsync(name);
            SystemManager.UI.MenuController.PopMenu();
            
        }

        public void OpenAssetPage()
        {
            
        }

        public void OpenDocs()
        {
            
        }

        public void OnSettingsButton()
        {
            SystemManager.UI.MenuController.OpenMenu("Setting Menu");
        }

        public void OnQuitButton()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
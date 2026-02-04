using System;
using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Resources.Script.UI
{
    public class PauseMenu : UIPopup,IView
    {
        public event Action ResumeClicked;
        public event Action SettingsClicked;
        public event Action MainMenuClicked;

        [SerializeField] private ButtonController resumeBtn;
        [SerializeField] private ButtonController settingsBtn;
        [SerializeField] private ButtonController mainMenuBtn;

        void Awake()
        {
            resumeBtn.onClick   += () => ResumeClicked?.Invoke();
            settingsBtn.onClick += () => SettingsClicked?.Invoke();
            mainMenuBtn.onClick += () => MainMenuClicked?.Invoke();
        }

        public void SetVisible(bool on) => gameObject.SetActive(on);
        
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
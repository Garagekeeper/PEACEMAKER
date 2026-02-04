using System;
using Resources.Script.Controller;
using UnityEngine;

namespace Resources.Script.UI
{
    public class MainMenu : MonoBehaviour, IView
    {
        public event Action StartClicked;
        public event Action SettingsClicked;
        public event Action ExitClicked;
        
        [SerializeField] private ButtonController startBtn;
        [SerializeField] private ButtonController settingsBtn;
        [SerializeField] private ButtonController exitBtn;

        void Awake()
        {
            startBtn.onClick   += () => StartClicked?.Invoke();
            settingsBtn.onClick += () => SettingsClicked?.Invoke();
            exitBtn.onClick += () => ExitClicked?.Invoke();
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
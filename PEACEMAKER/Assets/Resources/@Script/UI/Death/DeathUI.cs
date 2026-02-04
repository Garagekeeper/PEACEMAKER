using System;
using Resources.Script.Controller;
using UnityEngine;

namespace Resources.Script.UI.Death
{
    public class DeathUI : UIBase,IView
    {
        [SerializeField] private ButtonController gotoMainBtn;
        public event Action onGotoMain;

        private void Awake()
        {
            gotoMainBtn.onClick += OnGotoMainClicked;
        }

        private void OnGotoMainClicked()
        {
            onGotoMain?.Invoke();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            gotoMainBtn.onClick -= OnGotoMainClicked;
        }
    }
}
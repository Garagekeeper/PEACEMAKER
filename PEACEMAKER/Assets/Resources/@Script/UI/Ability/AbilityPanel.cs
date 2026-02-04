using System;
using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;
using static Resources.Script.Utilities;

namespace Resources.Script.UI.Ability
{
    public class AbilityPanel : UIPopup, IView
    {
        public AbilityCard[] abilities;
        public CanvasGroup canvasGroup;
        public RectTransform centerAnchor;
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}
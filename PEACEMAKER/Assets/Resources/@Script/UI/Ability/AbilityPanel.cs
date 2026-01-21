using System;
using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;
using static Resources.Script.Utilities;

namespace Resources.Script.UI.Ability
{
    public class AbilityPanel : MonoBehaviour, IView
    {
        public AbilityCard[] abilities;
        public CanvasGroup canvasGroup;
        public RectTransform centerAnchor;
        //public ResultUI resultUI;
        private bool _isLocked = false;
        public event Action<int> OnCardClicked;
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            for (int i = 0; i < abilities.Length; i++)
            {
                int index = i;
                abilities[i].Bind(() => OnCardClicked?.Invoke(index));
            }
        }
        
        public void LockInput()
        {
            foreach (var c in abilities) c.DisableInput();
        }
        
        public void PlayUnselectedDisappear(int selectedIndex, Action onAllDone)
        {
            int remain = 0;
            for (int i = 0; i < abilities.Length; i++)
            {
                if (i == selectedIndex) continue;

                abilities[i].PlayDisappearUnselected();
            }
            
            onAllDone?.Invoke();
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}
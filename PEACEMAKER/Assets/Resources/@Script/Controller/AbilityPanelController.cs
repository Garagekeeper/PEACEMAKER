using Resources.Script.Managers;
using UnityEngine;
using static Resources.Script.Utilities;

namespace Resources.Script.Controller
{
    public class AbilityPanelController : MonoBehaviour
    {
        public AbilityUIController[] abilities;
        public CanvasGroup canvasGroup;
        public RectTransform centerAnchor;
        //public ResultUI resultUI;
        private bool _isLocked = false;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            canvasGroup.alpha = 1f;
            SystemManager.Game.IsPaused = true;
            UnlockCursor();
            _isLocked = false;
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            SystemManager.Game.IsPaused = false;
            LockCursor();
        }

        public void OnAbilitySelected(AbilityUIController selected)
        {
            if (_isLocked)
                return;

            _isLocked = true;
            
            foreach (var ability in abilities)
            {
                ability.DisableInput();

                if (ability != selected)
                    ability.PlayDisappearUnselected(OnSelectedArrived);
                else
                    ability.ApplyAbility();
            }
        }
        
        void OnSelectedArrived()
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
            //StartCoroutine(FadeOutPanel());
        }
    }
}
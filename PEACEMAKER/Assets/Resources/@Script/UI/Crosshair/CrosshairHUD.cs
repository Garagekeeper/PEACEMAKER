using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Script.UI.Crosshair
{
    public class CrosshairHUD : MonoBehaviour, IView
    {
        public float size = 1;
        public float sizeMatchingTime = 0.1f;
        public RectTransform crosshairHolder;
        public Color color = Color.white;
        
        private CanvasGroup _canvasGroup;

        
        private float _amount;
        private float _progress;
        private float _spray;
        private float _target;
        private float _vel;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            ApplyColor(color);
        }

        private void Update()
        {
            if (HeadManager.Game.IsPaused)
            {
                _canvasGroup.alpha = 0;
                return;
            }

            _canvasGroup.alpha = Mathf.Lerp(1, 0, _progress * 1.3f);

            //현재 탄퍼짐 정도에 따라서 크로스헤어 확대 축소
            //float spray = Firearm.recoilAndSpray.GetCurrentSpray();
            //var target = Mathf.Clamp(spray, 1, 3);
            _amount = Mathf.SmoothDamp(_amount, _target, ref _vel, sizeMatchingTime);   
            crosshairHolder.sizeDelta = Vector2.one * size * _amount;
        }
        
        public void UpdateSprayValue(float sprayValue)
        {
            _spray = sprayValue;
            _target = Mathf.Clamp(_spray, 1, 3);
        }

        public void UpdateCrossHairProgress(float progress)
        {
            _progress = progress;
        }

        public void UpdateCrosshairColor(Color c)
        {
            color = c;
            ApplyColor(c);
        }

        private void ApplyColor(Color c)
        {
            foreach(Image image in crosshairHolder.GetComponentsInChildren<Image>())
            {
                image.color = c;
            }
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
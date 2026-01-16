using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Resources.Script.UI
{
    public class Crosshair : MonoBehaviour
    {
        public float size = 1;
        public float sizeMatchingTime = 0.1f;

        public Color color = Color.white;
        public RectTransform crosshairHolder;
        public FirearmController Firearm { get; set; }

        private float amount;
        private float sizeMatchingVel;

        private CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (HeadManager.Game.IsPaused)
            {
                canvasGroup.alpha = 0;
                return;
            }
            if(Firearm == null)
            {
                return;
            }

            foreach(Image image in crosshairHolder.GetComponentsInChildren<Image>())
            {
                image.color = color;
            }

            canvasGroup.alpha = Mathf.Lerp(1, 0, Firearm.anim.AimingAnimation.Progress * 1.3f);

            //현재 탄퍼짐 정도에 따라서 크로스헤어 확대 축소
            float spray = Firearm.recoilAndSpray.GetCurrentSpray();
            var target = Mathf.Clamp(spray, 1, 3);
            amount = Mathf.SmoothDamp(amount, target, ref sizeMatchingVel, sizeMatchingTime);   

            crosshairHolder.sizeDelta = Vector2.one * size * amount;
        }
    }
}
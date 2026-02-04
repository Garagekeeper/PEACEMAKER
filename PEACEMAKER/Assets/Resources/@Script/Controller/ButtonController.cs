using System;
using Resources.Script.Audio;
using Resources.Script.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Resources.Script.Controller
{
    public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool interactable = true;
        public Graphic targetGraphics;
        public TextMeshProUGUI targetText;
        public float fadeDuration = 0.1f;

        [Header("Graphics Colors")]
        public Color normalGraphicsColor = Color.black;
        public Color highlightedGraphicsColor = Color.white;
        public Color selectedGraphicsColor = Color.gray;
        public Color disabledGraphicsColor = Color.red;

        [Header("Text Colors")]
        public Color normalTextColor = Color.white;
        public Color highlightedTextColor = Color.black;
        public Color selectedTextColor = Color.black;
        public Color disabledTextColor = Color.black;

        [Header("Audio")]
        public AudioPreset highlightSound;
        public AudioPreset selectSound;

        [Space]
        public Action onClick;

        private Color currentGraphicsColor;
        private Color currentTextColor;
        

        public bool isHighlighted { get; private set; }
        public bool isPressed { get; private set; }

        private Color targetTextColor;
        private Color targetGraphicsColor;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            
        }

        protected void Update()
        {
            if (targetGraphics == null) return;

            if (interactable)
            {
                if (isHighlighted && !isPressed)
                {
                    currentGraphicsColor = highlightedGraphicsColor;
                    currentTextColor = highlightedTextColor;
                }

                if (isHighlighted && isPressed)
                {
                    currentGraphicsColor = selectedGraphicsColor;
                    currentTextColor = selectedTextColor;
                }

                if (!isHighlighted && !isPressed)
                {
                    currentGraphicsColor = normalGraphicsColor;
                    currentTextColor = normalTextColor;
                }
            }
            else
            {
                currentGraphicsColor = disabledGraphicsColor;
                currentTextColor = disabledTextColor;
            }

            targetGraphicsColor = Color.Lerp(targetGraphics.color, currentGraphicsColor, Time.unscaledDeltaTime / fadeDuration);
            targetTextColor = Color.Lerp(targetText.color, currentTextColor, Time.unscaledDeltaTime / fadeDuration);

            if (targetGraphics.color != targetGraphicsColor)
                targetGraphics.color = targetGraphicsColor;

            if (targetText.color != targetTextColor)
                targetText.color = targetTextColor;
        }

        void OnDisable()
        {
            isPressed = false;
            isHighlighted = false;
            currentGraphicsColor = normalGraphicsColor;
            currentTextColor = normalTextColor;
            targetGraphicsColor = currentGraphicsColor;
            targetTextColor = currentTextColor;
            targetGraphics.color = targetGraphicsColor;
            targetText.color = targetTextColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable == false) return;

            HeadManager.Audio.PlayWithPreset(highlightSound);

            isHighlighted = true;
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (interactable == false) return;

            HeadManager.Audio.PlayWithPreset(selectSound);

            isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (interactable == false) return;
            
            if (isHighlighted)
                onClick?.Invoke();

            isPressed = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (interactable == false) return;

            isHighlighted = false;
        }
        
        public void OnMenuButtonClicked()
        {
            
        }
        
        public void OnSettingButtonClicked()
        {
            //HeadManager.UI.MenuController.OpenMenu("Setting Menu");
        }
        
    }
}
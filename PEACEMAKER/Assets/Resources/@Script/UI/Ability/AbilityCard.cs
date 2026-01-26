using System;
using System.Collections;
using ChocDino.UIFX;
using Resources.Script.Ability;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Resources.Script.UI.Ability
{
    public class AbilityCard : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
    {
        
        public CanvasGroup canvasGroup;
        private RectTransform _rect;
        
        private GlowFilter _glowFilter;
        [FormerlySerializedAs("_upSpeed")]
        [Header("Hover Glowing")]
        [SerializeField] private float upSpeed = 8f;
        [SerializeField] private float downSpeed = 6f;
        [FormerlySerializedAs("_minValue")] 
        [SerializeField, Range(0f, 1f)] private float minValue = 0f;
        [FormerlySerializedAs("_maxValue")] [SerializeField, Range(0f, 1f)] private float maxValue = 1f;
        
        private bool _isHover;
        private float  _targetStrength;
        
        // Hover 기본 값
        private float _baseDistance = 128f;
        private float _baseEnergy = 8;
        private float _baseStrength = 0f;
        private Color _baseColor = Color.cyan;
        private Color _clickColor = Color.cyan;
        
        [SerializeField]private Image imageArea;
        [SerializeField]private TextMeshProUGUI textArea;
        
        private void Awake()
        {
            _glowFilter = GetComponent<GlowFilter>();
            canvasGroup = GetComponent<CanvasGroup>();
            //imageArea = GetComponentInChildren<Image>();
            textArea =  GetComponentInChildren<TextMeshProUGUI>();
            _rect = transform as RectTransform;
        }
        
        public Action onClick;

        private void OnEnable()
        {
            Init();
        }
        

        private void Update()
        {
            UpdateHoverGlowing();
        }

        public void SetCard(AbilityDef card)
        {
            SetImageArea(card.icon);
            SetTextArea(card.GetDescription());
        }

        private void SetTextArea(string text)
        {
            textArea.text = text;
        }

        private void SetImageArea(Sprite image)
        {
            imageArea.sprite = image;
        }

        public void Init()
        {
            _glowFilter.MaxDistance = _baseDistance;
            _glowFilter.ExpFalloffEnergy= _baseEnergy;
            _glowFilter.Strength = _baseStrength;
            _glowFilter.Color = _baseColor;
            canvasGroup.alpha = 1;
            _rect.localScale = Vector3.one;
        }
        
        /// <summary>
        ///  선택 못받은 카드들을 안보이게 처리하는 코루틴 실행
        /// </summary>
        /// <param name="onComplete"></param>
        public void PlayDisappearUnselected(System.Action onComplete = null)
        {
            StartCoroutine(DisappearUnselected(onComplete));
        }
        
        /// <summary>
        /// 카드에 마우스를 올리면, 호버링하는 효과를 업데이트 하는 함수
        /// </summary>
        private void UpdateHoverGlowing()
        {
            if (!_glowFilter || !_glowFilter.isActiveAndEnabled) return;
            
            float target = minValue;
            float dampSpeed = downSpeed;
            
            if (_isHover)
            {
                target = maxValue;
                dampSpeed = upSpeed;
            }
            
            //Strength 값을 조절헤 반짝임 정도 조절
            if (Mathf.Abs(_glowFilter.Strength - target) > 0.001f)
            {
                _glowFilter.Strength = MathUtils.DampTowards(_glowFilter.Strength, target, dampSpeed, Time.unscaledDeltaTime);
            }
        }
        
        /*------------------
         이벤트 함수
         -----------------*/
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHover = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(Ripple());
            StartCoroutine(ClickScale(GetComponent<RectTransform>()));
            onClick?.Invoke();
        }
        

        /// <summary>
        /// 선택받지 못한 카드들을 안보이게 처리하는 함수
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        IEnumerator DisappearUnselected(System.Action onComplete)
        {
            Vector3 startScale = _rect.localScale;
            Vector3 popScale = startScale * 1.1f;

            // 살짝의 pop (흔들림)
            float t = 0f;
            while (t < 0.1f)
            {
                _rect.localScale = Vector3.Lerp(startScale, popScale, t / 0.1f);
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            // 사라짐
            t = 0f;
            float duration = 0.25f;

            while (t < duration)
            {
                float lerp = t / duration;

                _rect.localScale = Vector3.Lerp(popScale, Vector3.zero, lerp);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, lerp);

                t += Time.unscaledDeltaTime;
                yield return null;
            }
            
            // 콜백 호출
            yield return new WaitForSecondsRealtime(0.15f);
            onComplete?.Invoke();
        }
       
        // 물결 효과
        IEnumerator Ripple()
        {
            float time = 0f;
            _glowFilter.EdgeSide = EdgeSide.Both;
            _glowFilter.Color = _clickColor;
            _glowFilter.MaxDistance = 0f;
            _glowFilter.Strength = 1f;

            while (time < 0.35f)
            {
                float t = time / 0.35f;

                // 거리 증가 → 물결 확산
                _glowFilter.MaxDistance = Mathf.Lerp(0f, 140f, t);

                // 가장자리만 남게 보이도록 약간 감쇠
                _glowFilter.ExpFalloffEnergy = Mathf.Lerp(16f, 8f, t);

                time += Time.unscaledDeltaTime;
                _glowFilter.EdgeSide = EdgeSide.Outside;
                yield return null;
            }

            // 정리
            _glowFilter.MaxDistance = 0f;
            _glowFilter.EdgeSide = EdgeSide.Outside;
            _isHover = false;
        }
        
        // 클릭시 떨림
        IEnumerator ClickScale(RectTransform rect)
        {
            Vector3 origin = Vector3.one;
            Vector3 pressed = origin * 0.96f;

            float t = 0f;
            while (t < 0.06f)
            {
                rect.localScale = Vector3.Lerp(origin, pressed, t / 0.06f);
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            t = 0f;
            while (t < 0.08f)
            {
                rect.localScale = Vector3.Lerp(pressed, origin, t / 0.08f);
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            rect.localScale = origin;
        }
        
        // mask와 함께 사용해서 돌면서 반짝이는 효과
        IEnumerator Loop()
        {
            RectTransform mask =  GetComponent<RectTransform>();
            RectTransform card =  GetComponent<RectTransform>();
            Vector2[] path =
            {
                new Vector2(0, card.rect.height / 2),   // 위
                new Vector2(card.rect.width / 2, 0),    // 오른쪽
                new Vector2(0, -card.rect.height / 2),  // 아래
                new Vector2(-card.rect.width / 2, 0)    // 왼쪽
            };

            int index = 0;
            while (true)
            {
                Vector2 start = mask.anchoredPosition;
                Vector2 target = path[index];

                float t = 0f;
                float dist = Vector2.Distance(start, target);
                float duration = dist / 300f;

                while (t < duration)
                {
                    mask.anchoredPosition = Vector2.Lerp(start, target, t / duration);
                    t += Time.unscaledDeltaTime;
                    yield return null;
                }

                mask.anchoredPosition = target;
                index = (index + 1) % path.Length;
            }
        }
        
    }
}

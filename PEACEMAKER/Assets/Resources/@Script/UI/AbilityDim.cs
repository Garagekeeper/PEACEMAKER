using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Script.UI
{
    public class AbilityDim : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Image background;

        Color normal;
        Color dim;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            background = GetComponent<Image>();

            normal = background.color;
            dim = normal * 0.5f;
            dim.a = normal.a;
        }

        public void Dim()
        {
            StartCoroutine(DimRoutine());
        }

        IEnumerator DimRoutine()
        {
            float t = 0f;
            float duration = 0.35f;

            while (t < duration)
            {
                float lerp = t / duration;

                canvasGroup.alpha = Mathf.Lerp(1f, 0f, lerp);
                background.color = Color.Lerp(normal, dim, lerp);

                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}
using Resources.Script.Managers;
using TMPro;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.UI.DamageEffect
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float lifeTime = 0.8f;
        [SerializeField] private float floatSpeed = 1.2f;

        private float t;
        private Color baseColor;
        
        public void Init(DamageInfo info)
        {
            text.text = info.amount.ToString();
            baseColor = Color.black;
            if (info.isCrit)
                baseColor =  Color.red;

            // 1. 위치 먼저 잡기
            transform.position = info.hitPoint + Vector3.up * 0.5f;
    
            // 2. 캔버스 월드 카메라 설정
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null) 
            {
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = 100; 
            }

            t = 0f;
        }

        private void Update()
        {
            t += Time.deltaTime;
            transform.position += Vector3.up * (floatSpeed * Time.deltaTime);
            
            float a = 1f - (t / 2 * lifeTime);
            var c = baseColor; 
            c.a = Mathf.Clamp01(a);
            text.color = c;
            
            // 카메라를 직접 바라보게 하되, 텍스트가 거울처럼 반전되지 않도록 처리
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, 
                Camera.main.transform.rotation * Vector3.up);
            
            if (t >= lifeTime)
                HeadManager.Resource.Destroy(gameObject);
        }
    }
}
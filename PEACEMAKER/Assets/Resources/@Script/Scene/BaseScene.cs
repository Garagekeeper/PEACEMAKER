using Resources.Script.Managers;
using Resources.Script.UI.Scene;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Script.Scene
{
    public class BaseScene : MonoBehaviour
    {
        private void Awake()
        {
            //TODO 임시
            HeadManager.Audio.ReSetting();
            Init();
            
        }
        
        /// <summary>
        /// 씬이 변환될때 초기화등의 동작을 할 때 사용
        /// </summary>
        public virtual void Init()
        {
            Object obj = GameObject.FindFirstObjectByType<EventSystem>();
            if (obj == null)
            {
                GameObject eventSystem = new GameObject("@EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
            HeadManager.Pool.ResetPool();
        }
    }
}
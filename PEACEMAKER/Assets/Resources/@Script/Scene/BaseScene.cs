using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Script.Scene
{
    public class BaseScene : MonoBehaviour
    {
        public virtual void Init()
        {
            Object obj = GameObject.FindFirstObjectByType<EventSystem>();
            if (obj == null)
            {
                GameObject eventSystem = new GameObject("@EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }
    }
}
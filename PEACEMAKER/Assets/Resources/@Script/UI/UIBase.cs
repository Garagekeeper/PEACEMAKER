using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Resources.Script.Defines;

namespace Resources.Script.UI
{
    public abstract class UIBase : MonoBehaviour 
    {
        public static void BindEvent(GameObject go, Action<PointerEventData> action = null, EUIEvent type = EUIEvent.Click)
        {
            UIEventHandler evt = go.GetComponent<UIEventHandler>();
            if (evt == null)
                evt = go.AddComponent<UIEventHandler>();

            switch (type)
            {
                case EUIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
                case EUIEvent.PointerDown:
                    evt.OnPointerDownHandler -= action;
                    evt.OnPointerDownHandler += action;
                    break;
                case EUIEvent.PointerUp:
                    evt.OnPointerUpHandler -= action;
                    evt.OnPointerUpHandler += action;
                    break;
                case EUIEvent.Drag:
                    evt.OnDragHandler -= action;
                    evt.OnDragHandler += action;
                    break;
            }
        }
    }
    
    
}
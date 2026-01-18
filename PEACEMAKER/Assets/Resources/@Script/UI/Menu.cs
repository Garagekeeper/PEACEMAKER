using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Resources.Script.UI
{
    public abstract class Menu : MonoBehaviour
    {
        public string MenuName;

        protected MenuController controller;

        protected virtual void Awake()
        {
            //controller = HeadManager.UI.MenuController;
        }

        /// <summary>
        /// 메뉴가 열릴 때 호출됨
        /// </summary>
        public virtual void OnOpen()
        {
            EventSystem.current.SetSelectedGameObject(null);
            InputSystem.QueueStateEvent(Mouse.current, new MouseState { position = new Vector2(0, 0) });
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 메뉴가 닫힐 때 호출됨
        /// </summary>
        public virtual void OnClose()
        {
            EventSystem.current.SetSelectedGameObject(null);
            gameObject.SetActive(false);
        }
    }
}
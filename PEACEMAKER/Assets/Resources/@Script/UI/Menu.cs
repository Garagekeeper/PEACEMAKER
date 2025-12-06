using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Script.UI
{
    public abstract class Menu : MonoBehaviour
    {
        public string MenuName;

        protected MenuController controller;

        protected virtual void Awake()
        {
            controller = SystemManager.UI.MenuController;
        }

        /// <summary>
        /// 메뉴가 열릴 때 호출됨
        /// </summary>
        public virtual void OnOpen()
        {
            EventSystem.current.SetSelectedGameObject(null);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 메뉴가 닫힐 때 호출됨
        /// </summary>
        public virtual void OnClose()
        {
            gameObject.SetActive(false);
        }
    }
}
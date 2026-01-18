using System.Collections.Generic;
using System.Linq;
using Resources.Script.Managers;
using UnityEngine;
using Resources.Script.UI;
using UnityEngine.SceneManagement;
using static Resources.Script.Utilities;

namespace Resources.Script.Controller
{
    public class MenuController : MonoBehaviour
    {
        public string DefaultMenu; // PauseMenu 이름 저장
        public Stack<Menu> MenuStack = new Stack<Menu>();
        public List<Menu> Menus = new List<Menu>();

        private bool IsMainMenuOpen =>
            MenuStack.Count > 0 && MenuStack.Peek() is MainMenu;

        void Awake()
        {
            //HeadManager.UI.MenuController = this;
            DefaultMenu = "Pause Menu";
        }

        private void Start()
        {
            //Menus = HeadManager.UI.UIRoot.GetComponentsInChildren<Menu>(true).ToList();

            // 게임 시작 시 MainMenu가 기본으로 열려야 한다면
            var startMenu = Menus.FirstOrDefault(m => m is MainMenu);
            if (startMenu != null && SceneManager.GetActiveScene().name.Contains("Main"))
                OpenMenu(startMenu);
        }

        private void Update()
        {
            UpdatePause();
        }

        public Menu FindMenu(string menuName)
        {
            return Menus.FirstOrDefault(m => m.MenuName == menuName);
        }

        public void OpenMenu(string menuName)
        {
            Menu menu = FindMenu(menuName);
            if (menu != null) OpenMenu(menu);
        }

        public void OpenMainMenu()
        {
            OpenMenu("Main Menu");
        }

        public void OpenMenu(Menu menu)
        {
            if (menu == null) return;

            // 현재 메뉴 비활성화
            if (MenuStack.Count > 0)
                MenuStack.Peek().OnClose();

            // 새 메뉴 Push
            MenuStack.Push(menu);
            menu.OnOpen();

            if (menu.MenuName == "Pause Menu")
            {
                HeadManager.Game.IsPaused = true;
                UnlockCursor();
            }
        }

        public void PopMenu()
        {
            if (MenuStack.Count == 0)
                return;

            // 현재 메뉴 닫기
            Menu top = MenuStack.Pop();
            if (top.MenuName == "Pause Menu")
            {
                HeadManager.Game.IsPaused = false;
                LockCursor();
            }
            top.OnClose();

            // 이전 메뉴 되돌리기
            if (MenuStack.Count > 0)
            {
                MenuStack.Peek().OnOpen();
            }
        }

        public void CloseAll()
        {
            while (MenuStack.Count > 0)
            {
                MenuStack.Pop().OnClose();
            }
        }

        public void UpdatePause()
        {
            if (HeadManager.Input.State.PauseState)
            {
                // 메인메뉴가 열려 있다면 PauseMenu를 띄우면 안 됨
                if (IsMainMenuOpen)
                    return;
                
                if (MenuStack.Count == 0)
                {
                    Menu menu = FindMenu(DefaultMenu);
                    if (menu)
                        OpenMenu(menu);
                }
                else
                {
                    PopMenu();
                }
            }
        }
        
        public void OnPauseInput()
        {
            // ESC 눌렀을 때

            // 메인메뉴가 열려 있다면 PauseMenu를 띄우면 안 됨
            if (IsMainMenuOpen)
                return;

            if (MenuStack.Count == 0)
            {
                Menu menu = FindMenu(DefaultMenu);
                if (menu != null)
                    OpenMenu(menu);
            }
            else
            {
                PopMenu();
            }
        }
    }
}
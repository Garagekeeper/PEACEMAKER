using System;
using System.Collections;
using System.Collections.Generic;
using Resources.Script.Ability;
using Resources.Script.Creatures;
using Resources.Script.Managers;
using UnityEngine;
using static Resources.Script.Utilities;
namespace Resources.Script.UI.Ability
{
    public class AbilityPanelPresenter : Presenter<AbilityPanel, Player>
    {
        private bool _locked = false;
        private List<AbilityDef> _abilities = new();
        
        public AbilityPanelPresenter(AbilityPanel view, Player model) : base(view, model)
        {
            
        }
        
        public override void Init()
        {
            model.OnLevelUp     += OnLevelUp;
            model.OnLevelUpDone += OnLevelUpDone;

            for (var i = 0; i < view.abilities.Length; i++)
            {
                var index = i;
                view.abilities[i].onClick += () => OnCardClicked(index);
            }
        }

        
        /// <summary>
        /// 레벨업을 시작할 때
        /// </summary>
        private void OnLevelUp()
        {
            HeadManager.Game.IsPaused = true;
            _locked = false;
            
            LockInput(); // 일단 잠금
            UnlockCursor();
            SetThreeCard();
            HeadManager.UI.OpenPopup(view);
            view.Show();
            CaraAppearAnim();
        }

        private async void CaraAppearAnim()
        {
            try
            {
                // 0) 카드마다 delay step 만큼의 간격으로 애니메이션 실행
                float delayStep = 0.15f;
                for (int i = 0; i < view.abilities.Length; i++)
                {
                    view.abilities[i].PlayAppear(i * delayStep);
                }
            
                // 1) 모든 애니메이션이 끝나기를 기다림
                // 연출길이 * 3 + @;
                int totalWaitMs = (int)(((view.abilities.Length - 1) * delayStep + 0.6f) * 1000);
                await System.Threading.Tasks.Task.Delay(totalWaitMs);

                // 2) 클릭 허용
                UnLockInput();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void SetThreeCard()
        {
            _abilities = HeadManager.Ability.SelectThreeAbilities();
            for (var i = 0; i < 3; i++)
            {
                view.abilities[i].SetCard(_abilities[i]);
            }
        }
        
        /// <summary>
        /// 레벨업을 종료할 때
        /// </summary>
        private void OnLevelUpDone()
        {
            view.Hide();
            HeadManager.UI.PopPopup();
            HeadManager.Game.IsPaused = false;
            LockCursor();
        }

        private void LockInput() { for (var i = 0; i < 3; i++) DisableInput(i); }
        private void UnLockInput() { for (var i = 0; i < 3; i++) EnableInput(i); }
        
        /// <summary>
        /// 해당 카드가 사용자의 입력을 받지 않도록 처리하는 함수
        /// </summary>
        private void DisableInput(int index)
        {
            view.abilities[index].canvasGroup.interactable = false;
            view.abilities[index].canvasGroup.blocksRaycasts = false;
        }
        
        /// <summary>
        /// 해당 카드가 사용자의 입력을 받지도록 처리하는 함수
        /// </summary>
        private void EnableInput(int index)
        {
            view.abilities[index].canvasGroup.interactable = true;
            view.abilities[index].canvasGroup.blocksRaycasts = true;
        }
        
        /// <summary>
        /// 카드가 클릭되면 실행될 함수
        /// </summary>
        /// <param name="index"></param>
        private void OnCardClicked(int index)
        {
            if (_locked) return;
            _locked = true;

            LockInput();

            // 1) 능력 적용 (모델에 요청)
            var target      = _abilities[index].target;
            var id       = _abilities[index].id;
            var op = _abilities[index].op;
            var val      = _abilities[index].GetFinalValue();
            model.ApplyAbility(target, id, op, val);

            // 2) UI 연출 후 닫기
            // 연출이 끝날때까지 대기
            PlayUnselectedDisappear(index);
            

        }

        /// <summary>
        /// 선택받지 못한 카드를 없애는 함수
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void PlayUnselectedDisappear(int selectedIndex)
        {
            List<Coroutine> coroutines = new();
            for (int i = 0; i < view.abilities.Length; i++)
            {
                if (i == selectedIndex) continue;

                coroutines.Add(view.abilities[i].PlayDisappearUnselected());
            }

            // view의 모노를 끌어와서 사용
            view.StartCoroutine(WaitCardDisappearAnim(coroutines));
        }

        private IEnumerator WaitCardDisappearAnim(List<Coroutine> coroutines)
        {
            foreach (var c in coroutines)
            {
                yield return c;
            }
            model.EndLevelUp();
        }
        
        public override void Release()
        {
            model.OnLevelUp     -= OnLevelUp;
            model.OnLevelUpDone -= OnLevelUpDone;
            
            for (var i = 0; i < view.abilities.Length; i++)
            {
                var index = i;
                view.abilities[i].onClick -= () => OnCardClicked(index);
            }
        }
    }
}
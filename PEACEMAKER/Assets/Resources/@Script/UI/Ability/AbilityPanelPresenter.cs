using Resources.Script.Creatures;
using Resources.Script.Managers;
using static Resources.Script.Utilities;

namespace Resources.Script.UI.Ability
{
    public class AbilityPanelPresenter : Presenter<AbilityPanel, Player>
    {
        private bool _locked = false;
        public AbilityPanelPresenter(AbilityPanel view, Player model) : base(view, model)
        {
            
        }
        
        public override void Init()
        {
            model.OnLevelUp     += OnLevelUp;
            model.OnLevelUpDone += OnLevelUpDone;
            view.OnCardClicked  += OnCardClicked;
        }

        
        /// <summary>
        /// 레벨업을 시작할 때
        /// </summary>
        private void OnLevelUp()
        {
            HeadManager.Game.IsPaused = true;
            _locked = false;
            UnlockCursor();
            view.Show();
        }

        
        /// <summary>
        /// 레벨업을 종료할 때
        /// </summary>
        private void OnLevelUpDone()
        {
            view.Hide();
            HeadManager.Game.IsPaused = false;
            LockCursor();
        }
        
        /// <summary>
        /// 카드가 클릭되면 실행될 함수
        /// AbilityPanel의 OnCardClicked에 등록되어있음.
        /// </summary>
        /// <param name="index"></param>
        private void OnCardClicked(int index)
        {
            if (_locked) return;
            _locked = true;

            view.LockInput();

            // 1) 능력 적용 (모델에 요청)
            model.ApplyAbility(index); // <- Player 쪽에 메서드 준비

            // 2) UI 연출 후 닫기
            view.PlayUnselectedDisappear(index, () => { model.EndLevelUp(); });
        }
        
        public override void Release()
        {
            model.OnLevelUp     -= OnLevelUp;
            model.OnLevelUpDone -= OnLevelUpDone;
            view.OnCardClicked -= OnCardClicked;
        }
    }
}
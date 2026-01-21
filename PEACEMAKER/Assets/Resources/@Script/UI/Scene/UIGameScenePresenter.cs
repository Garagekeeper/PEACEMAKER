using Resources.Script.Controller;
using Resources.Script.Creatures;
using Resources.Script.UI.Ability;
using Resources.Script.UI.Crosshair;
using Resources.Script.UI.Firearm;
using Unity.InferenceEngine;

namespace Resources.Script.UI.Scene
{
    public class InGameScenePresenter : Presenter<UIGameScene, Player>
    {
        private FirearmHUDPresenter _firearmPresenter;
        private PlayerCardHUDPresenter _playerCardPresenter;
        private AbilityPanelPresenter _abilityPanelPresenter;

        public InGameScenePresenter(UIGameScene view, Player player) : base(view, player) { }
        public override void Init()
        {
            // 자식 Presenter들을 생성하여 조립합니다.
            _firearmPresenter = new FirearmHUDPresenter(view.FirearmHUD, model.PController.Inventory);
            _playerCardPresenter = new PlayerCardHUDPresenter(view.PlayerCardHUD, model);
            _abilityPanelPresenter =  new AbilityPanelPresenter(view.AbilityPanel, model);
            
            // 자식들도 초기화 시켜줍니다.
            _firearmPresenter.Init();
            _playerCardPresenter.Init();
            _abilityPanelPresenter.Init();
        }
        

        public override void Release()
        {

        }
    }
}
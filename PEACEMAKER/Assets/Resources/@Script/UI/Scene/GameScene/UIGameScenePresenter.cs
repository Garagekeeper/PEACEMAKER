using Resources.Script.Controller;
using Resources.Script.Creatures;
using Resources.Script.Managers;
using Resources.Script.UI.Ability;
using Resources.Script.UI.Crosshair;
using Resources.Script.UI.Death;
using Resources.Script.UI.Firearm;
using Resources.Script.UI.Setting;
using Unity.InferenceEngine;

namespace Resources.Script.UI.Scene
{
    public class InGameScenePresenter : Presenter<UIGameScene, Player>
    {
        private FirearmHUDPresenter _firearmPresenter;
        private PlayerCardHUDPresenter _playerCardPresenter;
        private AbilityPanelPresenter _abilityPanelPresenter;
        private PauseMenuPresenter _pauseMenuPresenter;
        private SettingMenuPresenter _settingMenuPresenter;
        private DeathUIPresenter _deathUIPresenter;

        public InGameScenePresenter(UIGameScene view, Player player) : base(view, player) { }
        public override void Init()
        {
            // 자식 Presenter들을 생성하여 조립.
            _firearmPresenter = new FirearmHUDPresenter(view.FirearmHUD, model.PController.Inventory);
            _playerCardPresenter = new PlayerCardHUDPresenter(view.PlayerCardHUD, model);
            _abilityPanelPresenter =  new AbilityPanelPresenter(view.AbilityPanel, model);
            _pauseMenuPresenter = new PauseMenuPresenter(view.PauseMenu, HeadManager.Game);
            _settingMenuPresenter = new SettingMenuPresenter(view.SettingsMenu, HeadManager.Game);
            _deathUIPresenter = new  DeathUIPresenter(view.DeathUI, HeadManager.Game);
            
            // 자식들도 초기화.
            _firearmPresenter.Init();
            _playerCardPresenter.Init();
            _abilityPanelPresenter.Init();
            _pauseMenuPresenter.Init();
            _settingMenuPresenter.Init();
            _deathUIPresenter.Init();
        }
        

        public override void Release()
        {
            _firearmPresenter.Release();
            _playerCardPresenter.Release();
            _abilityPanelPresenter.Release();
            _pauseMenuPresenter.Release();
            _settingMenuPresenter.Release();
            _deathUIPresenter.Release();
        }
    }
}
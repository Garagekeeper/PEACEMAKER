using Resources.Script.Creatures;
using Resources.Script.UI.PlayerCard;

namespace Resources.Script.UI.Crosshair
{
    public class PlayerCardHUDPresenter : Presenter<PlayerCardHUD, Player>
    {
        public PlayerCardHUDPresenter(PlayerCardHUD view, Player model) : base(view, model)
        {
            
        }

        public override void Init()
        {   
            model.onHpChange      += UpdateHpValue;
            model.onExpChange     += UpdateExpValue;
            model.OnLevelUp       += UpdateLevelUpStateOn;
            model.OnLevelUpDone   += UpdateLevelUpStateOff;
        }

        private void UpdateHpValue(float value, float maxValue)
        {
            view.UpdateHpValue(value, maxValue);
        }
        
        private void UpdateExpValue(float value, float maxValue)
        {
            view.UpdateExpValue(value, maxValue);
        }

        private void UpdateLevelUpStateOn()
        {
            view.UpdateLevelUpState(true);
        }
        
        private void UpdateLevelUpStateOff()
        {
            view.UpdateLevelUpState(false);
        }
        
        public override void Release()
        {
            model.onHpChange      -= UpdateHpValue;
            model.onExpChange     -= UpdateExpValue;
            model.OnLevelUp       -= UpdateLevelUpStateOn;
            model.OnLevelUpDone   -= UpdateLevelUpStateOff;
        }
    }
}
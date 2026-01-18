using UnityEngine;
using Resources.Script.UI.Firearm;
using Resources.Script.UI.Crosshair;
using Resources.Script.UI.PlayerCard;
using UnityEngine.Serialization;

namespace Resources.Script.UI.Scene
{
    public class UIGameScene : UIScene, IView
    {
        [SerializeField] private FirearmHUD firearmHUD = null;
        [SerializeField] private CrosshairHUD crosshairHUD = null;
        [SerializeField] private PlayerCardHUD playerCardHUD = null;
        [SerializeField] private VisualizedHpEffect hpEffect = null;
        
        public FirearmHUD FirearmHUD => firearmHUD;
        public CrosshairHUD CrosshairHUD => crosshairHUD;
        public PlayerCardHUD PlayerCardHUD => playerCardHUD;
        public VisualizedHpEffect HpEffect => hpEffect;

        public void SetCrosshairSpray(float value)
        {
            crosshairHUD.UpdateSprayValue(value);
        }

        public void SetCrossHairProgress(float value)
        {
            crosshairHUD.UpdateCrossHairProgress(value);
        }

        public void TriggerDamageEffect()
        {
            hpEffect.TriggerDamageEffect();
        }

        public void TriggerHealingEffect()
        {
            hpEffect.TriggerHealingEffect();
        }
        
        public void Show()
        {
            
        }

        public void Hide()
        {
            
        }
    }
}
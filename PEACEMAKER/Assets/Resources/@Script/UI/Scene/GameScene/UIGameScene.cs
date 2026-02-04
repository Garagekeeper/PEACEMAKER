using System;
using Resources.Script.Managers;
using Resources.Script.UI.Ability;
using UnityEngine;
using Resources.Script.UI.Firearm;
using Resources.Script.UI.Crosshair;
using Resources.Script.UI.DamageEffect;
using Resources.Script.UI.PlayerCard;
using Resources.Script.UI.Setting;
using Resources.Script.UI.Death;
using static  Resources.Script.Defines;

using UnityEngine.Serialization;

namespace Resources.Script.UI.Scene
{
    public class UIGameScene : UIScene, IView
    {
        [SerializeField] private FirearmHUD firearmHUD = null;
        [SerializeField] private CrosshairHUD crosshairHUD = null;
        [SerializeField] private PlayerCardHUD playerCardHUD = null;
        [SerializeField] private VisualizedHpEffect hpEffect = null;
        [SerializeField] private AbilityPanel abilityPanel = null;
        [SerializeField] private PauseMenu pauseMenu = null;
        [SerializeField] private SettingMenu settingMenu = null;
        [SerializeField] private DeathUI deathUI = null;
        [SerializeField] private Hitmarker hitmarker = null;
        
        public FirearmHUD FirearmHUD => firearmHUD;
        public CrosshairHUD CrosshairHUD => crosshairHUD;
        public PlayerCardHUD PlayerCardHUD => playerCardHUD;
        public VisualizedHpEffect HpEffect => hpEffect;
        public AbilityPanel AbilityPanel => abilityPanel;
        public PauseMenu PauseMenu => pauseMenu;
        public SettingMenu SettingsMenu => settingMenu;
        public DeathUI DeathUI => deathUI;
        public Hitmarker Hitmarker => hitmarker;

        private void Update()
        {
            if (HeadManager.Input.State.PauseState)
                HandleEsc();
        }

        public void HandleEsc()
        {
            HeadManager.Game.HandleEsc();
        }

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

        public void UpdateAbilityPanel(bool state)
        {
            if (state)
                abilityPanel.Show();
            else 
                abilityPanel.Hide();
        }

        public void CloseAbilityPanel()
        {
            
        }

        public void ShowDamageText(DamageInfo info)
        {
            var go = HeadManager.Resource.Instantiate(EObjectID.DMGFX, HeadManager.ObjManager.DmgRoot);
            go.GetComponent<DamageText>().Init(info);
        }

        public void ShowHitmarker(DamageInfo info)
        {
            Hitmarker.Show(info.isCrit);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
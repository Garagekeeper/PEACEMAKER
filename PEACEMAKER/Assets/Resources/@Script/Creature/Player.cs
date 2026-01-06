using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEditor;
using UnityEngine;
using static Resources.Script.Defines;
namespace Resources.Script.Creature
{
    public class Player : DamageableObject
    {
        public PlayerController PController { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Player;
            SystemManager.Game.MainPlayer = this;
            PController = GetComponent<PlayerController>();
        }

        protected override void Start()
        {
            SystemManager.UI.AddPlayerCard(this);
        }
        
        protected override void Update()
        {
            SystemManager.UI.PlayerCardHUDIns.UpdateCard();
        }

        public override void OnDeath()
        {
            Debug.Log("Player Death");
        }

        public override void OnDamage(float value, Creature attackBy, bool isCrit = false)
        {
            base.OnDamage(value, attackBy);
            SystemManager.UI.HpEffect.TriggerDamageEffect();
        }

        public override void GetKill()
        {
            base.GetKill();
            LevelUp();
            
        }

        public void LevelUp()
        {
            // 0. 경험치통 증가
            // 1. 능력 선택 UI 호출
            SystemManager.UI.OnOffAbilityUI(true);
        }

        
        
    }
}
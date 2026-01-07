using System;
using Resources.Script.Controller;
using Resources.Script.InteractiveObject;
using Resources.Script.Managers;
using UnityEngine;
using static Resources.Script.Defines;
namespace Resources.Script.Creatures
{
    public class Player : DamageableObject, IPickupCollector
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
        }

        private void LevelUp()
        {
            // 0. 경험치통 증가
            // 1. 능력 선택 UI 호출
            SystemManager.UI.OnOffAbilityUI(true);
        }
            

        private void OnTriggerEnter(Collider other)
        {
            // 상대가 Pickup 아이템이고, 내가 픽업을 수집하면
            if (other.transform.TryGetComponent<IPickup>(out var pickup) &&
                TryGetComponent<IPickupCollector>(out var collector))
            {
                // 수집처리
                collector.Collect(pickup);
                pickup.OnPickedUp();
            }
        }

        public void Collect(IPickup pickup)
        {
            if (pickup is ExpGem gem)
            {
                AddGem();
            }
        }

        private void AddGem()
        {
            LevelUp();
        }
    }
}
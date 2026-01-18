using System;
using Resources.Script.Controller;
using Resources.Script.InteractiveObject;
using Resources.Script.Managers;
using Resources.Script.UI.Scene;
using UnityEngine;
using static Resources.Script.Defines;
namespace Resources.Script.Creatures
{
    public class Player : DamageableObject, IPickupCollector
    {
        [Header("Exp")] 
        [SerializeField] private int maxExp;
        [SerializeField] private int currExp;
        public int CurrExp
        {
            get => currExp;
            set
            {
                if (currExp == value) return;
                
                currExp = Math.Clamp(value, 0, MaxExp);
                if (currExp == MaxExp)
                    LevelUp();
                onExpChange?.Invoke(value, MaxExp);
            }
        }

        public int MaxExp
        {
            get=>maxExp;
            private set => maxExp = value;
        }

        protected override void NotifyHpChanged()
        {
            onHpChange?.Invoke(Hp, MaxHp);
        }
        
        
        public PlayerController PController { get; private set; }
        
        public Action<float, float> onExpChange;
        public Action<float, float> onHpChange;
        
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Player;
            HeadManager.Game.MainPlayer = this;
            PController = GetComponent<PlayerController>();
            //TODO 경험치량 조절
            MaxExp = 20;
            ObjectType = EObjectType.Player;
        }

        protected override void Start()
        {
            
        }
        
        protected override void Update()
        {
            
        }

        public override void OnDeath()
        {
            Debug.Log("Player Death");
        }

        public override void OnDamage(float value, Creature attackBy, bool isCrit = false)
        {
            base.OnDamage(value, attackBy ,isCrit);
            if (HeadManager.UI.SceneUI is UIGameScene)
            {
                var temp = (UIGameScene)HeadManager.UI.SceneUI;
                temp.TriggerDamageEffect();
            }
        }

        public override void GetKill()
        {
            
        }

        private void LevelUp()
        {
            // 0. 경험치통 증가
            // 1. 능력 선택 UI 호출
            CurrExp = 0;
        }
            

        private void OnTriggerEnter(Collider other)
        {
            // 상대가 Pickup 아이템이고, 내가 픽업을 수집하면
            if (other.transform.TryGetComponent<IPickup>(out var pickup))
            {
                // 수집처리
                Collect(pickup);
                // pickup에서 수집시 함수 호출
                pickup.OnPickedUp();
            }
        }

        public void Collect(IPickup pickup)
        {
            // 0. ExpGem이면 Gem 획득처리
            if (pickup is ExpGem gem)
            {
                AddGem(gem);
            }
        }

        /// <summary>
        /// ExpGem획득 처리 함수
        /// </summary>
        /// <param name="gem"></param>
        private void AddGem(ExpGem gem)
        {
            int expAmount = 0;
            // 0. Gem의 희귀도에 따라서 경험치양 결정
            switch (gem.Rarity)
            {
                case ERarity.Normal:
                    expAmount = 10;
                    break;
                case ERarity.Rare:
                    expAmount = 20;
                    break;
                case ERarity.Epic:
                    expAmount = 50;
                    break;
            }
            
            // 1. 경험치 획득
            GetExp(expAmount);
        }

        /// <summary>
        /// 경험치 획득 함수
        /// </summary>
        /// <param name="expAmount"></param>
        private void GetExp(int expAmount)
        {
            // 프로퍼티 내부에서 클램프, 레벨업 다 해줌.
            CurrExp += expAmount;
        }
    }
}
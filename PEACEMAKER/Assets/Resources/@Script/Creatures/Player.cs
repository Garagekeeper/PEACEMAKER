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
        [Header("Exp")] [SerializeField] private int maxExp;
        [SerializeField] private int currExp;

        public int CurrExp
        {
            get => currExp;
            set => currExp = value;
        }

        public int MaxExp
        {
            get => maxExp;
            private set => maxExp = value;
        }

        protected override void NotifyHpChanged()
        {
            onHpChange?.Invoke(Hp, MaxHp);
        }


        public PlayerController PController { get; private set; }

        public Action<float, float> onExpChange;
        public Action OnLevelUp;
        public Action OnLevelUpDone;
        public Action<float, float> onHpChange;

        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Player;
            HeadManager.Game.MainPlayer = this;
            PController = GetComponent<PlayerController>();
            //TODO 경험치량 조절
            MaxExp = 10;
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
            base.OnDamage(value, attackBy, isCrit);
            if (HeadManager.UI.SceneUI is UIGameScene)
            {
                var temp = (UIGameScene)HeadManager.UI.SceneUI;
                temp.TriggerDamageEffect();
            }
        }

        public override void GetKill()
        {

        }


        // private void OnTriggerEnter(Collider other)
        // {
        //     // 상대가 Pickup 아이템이고, 내가 픽업을 수집하면
        //     if (other.transform.TryGetComponent<IPickup>(out var pickup))
        //     {
        //         // 수집처리
        //         Collect(pickup);
        //         // pickup에서 수집시 함수 호출
        //         pickup.OnPickedUp();
        //     }
        // }

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
            // 0. 경험치 획득
            GetExp(gem.ExpAmount);
        }

        /// <summary>
        /// 경험치 획득 함수
        /// </summary>
        /// <param name="expAmount"></param>
        private void GetExp(int expAmount)
        {
            currExp += expAmount;

            // 경험치가 가득 찼을 때 (연속 레벨업 대응을 위해 while 사용)
            while (currExp >= maxExp)
            {
                LevelUp();
            }

            onExpChange?.Invoke(currExp, maxExp);
        }

        private void LevelUp()
        {
            currExp -= maxExp; // 잔여 경험치 보존

            // 0. 경험치통 증가 (예: 기존의 20% 증가)
            maxExp = Mathf.RoundToInt(maxExp * 1.2f);

            // 1. 게임 일시 정지
            Time.timeScale = 0f;

            // 2. 카드 UI 호출 알림
            OnLevelUp?.Invoke();
        }

        public void ApplyAbility(int index)
        {
            print("TODO : 능력 적용 구현하기");
        }

        public void EndLevelUp()
        {
            OnLevelUpDone?.Invoke();
            Time.timeScale = 1f;
        }
        
    }
}
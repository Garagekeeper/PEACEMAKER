using System;
using System.Collections;
using Resources.Script.Controller;
using Resources.Script.InteractiveObject;
using Resources.Script.Managers;
using Resources.Script.UI.Scene;
using UnityEngine;
using static Resources.Script.Defines;
using static Resources.Script.Utilities;
namespace Resources.Script.Creatures
{
    public class Player : DamageableObject, IPickupCollector, IMovable
    {
        [Header("Exp")] [SerializeField] private int maxExp;
        [SerializeField] private int currExp;
        private int _pendingLevelUps;
        private bool _isLevelUpUIOpen = false;

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

        public float Speed { get; set; } = 5f;
        public float SpeedMultiplier { get; set; } = 1f;

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
            base.GetKill();
            //TODO 킬 관련 로직
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

            // 레벨업 횟수만 누적 (UI는 한 번만 연다)
            while (currExp >= maxExp)
            {
                currExp -= maxExp;
                maxExp = Mathf.RoundToInt(maxExp * 1.2f);
                _pendingLevelUps++;
            }
            
            // 아직 UI 안 열려있고, 레벨업 대기분이 있으면 시작
            if (_pendingLevelUps > 0 && !_isLevelUpUIOpen)
            {
                LevelUp();
            }

            onExpChange?.Invoke(currExp, maxExp);
        }

        private void LevelUp()
        {
            // 1. 게임 일시 정지
            Time.timeScale = 0f;

            
            // 2. 카드 UI 호출 알림
            _isLevelUpUIOpen = true;
            OnLevelUp?.Invoke();
        }

        private IEnumerator OpenNextFrame()
        {
            // timescale=0 이어도 한 프레임 넘기려면 unscaled로 기다려야 함
            yield return null;
            LevelUp();
        }


        /// <summary>
        /// 결정된 능력을 적용하는 함수, special의 경우 별도의 함수 연결
        /// </summary>
        /// <param name="target">능력을 적용할 타켓</param>
        /// <param name="id">능력의 id</param>
        /// <param name="op">연산자 (스탯 변경시 사용)</param>
        /// <param name="val">값 (스탯 변경시 사용)</param>
        public void ApplyAbility(EAbilityTarget target,  int id, EOperator op = EOperator.None, float val = 0)
        {
            if (target == EAbilityTarget.Special)
            {
                //TODO
                //특별 능력을 위한 함수연결
                return;
            }

            //너무 얽혀있음. 그냥 이벤트로 넘기든 뭐든 수정해야할듯
            switch (target)
            {
                case EAbilityTarget.Ammo:
                    PController.Inventory.GetCurrentItem().AmmoInMagazine =
                        Mathf.RoundToInt(ChangeValue(PController.Inventory.GetCurrentItem().AmmoInMagazine, op, val));
                    break;
                case EAbilityTarget.Damage:
                    DamageMultiplier = ChangeValue(DamageMultiplier, op, val);
                    break;
                case EAbilityTarget.Hp:
                    if (Mathf.Approximately(Hp, MaxHp))
                        MaxHp =  ChangeValue(MaxHp, op, val);
                    Hp = ChangeValue(Hp, op, val);
                    break;
                case EAbilityTarget.Speed:
                    SpeedMultiplier = ChangeValue(SpeedMultiplier, op, val);
                    break;
                case EAbilityTarget.Spray:
                    SprayMultiplier = ChangeValue(SprayMultiplier, op, val);
                    break;
            }
            
            
            
        }

        public void EndLevelUp()
        {
            OnLevelUpDone?.Invoke();
            Time.timeScale = 1f;
            
            _pendingLevelUps--;

            if (_pendingLevelUps > 0)
            {
                // 다음 레벨업을 "다음 프레임"에 열어라 (UI 갱신/애니메이션 안전)
                StartCoroutine(OpenNextFrame());
            }
            else
            {
                _isLevelUpUIOpen = false;
                Time.timeScale = 1f; // 재개
            }
        }

        
    }
}
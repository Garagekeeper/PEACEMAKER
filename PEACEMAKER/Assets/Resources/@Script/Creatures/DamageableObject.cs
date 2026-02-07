using System;
using UnityEngine;
using UnityEngine.Events;

namespace Resources.Script.Creatures
{
    public abstract class DamageableObject : Creature, IDamageable
    {
        [SerializeField] protected float maxHp = 100f;
        [SerializeField] protected UnityEvent onDeathEvent;

        public float DamageMultiplier { get; set; } = 1f;
        public float SprayMultiplier { get; set; } = 1f;
        
        
        private float _hp;
        public float Hp
        {
            get => _hp;
            protected set
            {
                if (Mathf.Approximately(_hp, value)) return;
                _hp = value;
                
                NotifyHpChanged();
            }
        }

        protected float MaxHp
        {
            get => maxHp;
            set =>  maxHp = value;
        }
        
        protected virtual void NotifyHpChanged()
        {
            
        }
        
        public bool IsDead => Hp <= 0;

        protected override void Awake()
        {
            base.Awake();
            Hp = maxHp;
            
        }

        public virtual void OnDamage(float value, Creature attackBy, Vector3 hitPos, bool isCrit = false)
        {
            if (IsInvincible) return;
            if (IsDead) return;
            Hp = Mathf.Max(0, Hp - value);
            if (Hp == 0) HandleDeath(attackBy);
        }

        protected virtual void HandleDeath(Creature attackBy)
        {
            attackBy?.GetKill();
            OnDeath();
        }

        public virtual void OnDeath()
        {
            // 미니맵 마커 끄기
            if (_minimapMarkerRenderer != null) _minimapMarkerRenderer.enabled = false;
            onDeathEvent?.Invoke();
        }
        
        public void Heal(float amount)
        {
            if (IsDead) return;
            Hp = Mathf.Min(maxHp, Hp + amount);
        }

        

        
    }
}
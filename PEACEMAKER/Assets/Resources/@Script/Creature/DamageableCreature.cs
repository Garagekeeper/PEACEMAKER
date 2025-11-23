using UnityEngine;
using UnityEngine.Events;

namespace Resources.Script.Creature
{
    public abstract class DamageableCreature : Resources.Script.Creature.Creature, IDamageable
    {
        [SerializeField]protected float maxHp = 100f;
        [SerializeField] protected UnityEvent onDeathEvent;
        
        public float Hp { get; protected set; }
        public bool IsDead => Hp <= 0;

        protected override void Awake()
        {
            base.Awake();
            Hp = maxHp;
        }
        
        public void OnDamage(float value, Resources.Script.Creature.Creature attackBy)
        {
            if (IsDead) return;
            Hp = Mathf.Max(0, Hp - value);
            if (Hp == 0) HandleDeath(attackBy);
        }

        protected virtual void HandleDeath(Resources.Script.Creature.Creature attackBy)
        {
            attackBy?.GetKill();
            OnDeath();
        }

        public void OnDeath()
        {
            onDeathEvent?.Invoke();
        }
        
        public void Heal(float amount)
        {
            if (IsDead) return;
            Hp = Mathf.Min(maxHp, Hp + amount);
        }
    }
}
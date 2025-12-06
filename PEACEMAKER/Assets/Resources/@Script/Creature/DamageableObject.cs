using UnityEngine;
using UnityEngine.Events;

namespace Resources.Script.Creature
{
    public abstract class DamageableObject : Creature, IDamageable
    {
        [SerializeField]protected float maxHp = 100f;
        [SerializeField] protected UnityEvent onDeathEvent;
        private RagdollEffect _ragdollEffect;

        public float Hp { get; protected set; }
        public bool IsDead => Hp <= 0;

        private void Start()
        {
            _ragdollEffect = GetComponent<RagdollEffect>();
        }

        protected override void Awake()
        {
            base.Awake();
            Hp = maxHp;
        }
        
        public virtual void OnDamage(float value, Creature attackBy)
        {
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
            _ragdollEffect.Enable();
            onDeathEvent?.Invoke();
        }
        
        public void Heal(float amount)
        {
            if (IsDead) return;
            Hp = Mathf.Min(maxHp, Hp + amount);
        }
    }
}
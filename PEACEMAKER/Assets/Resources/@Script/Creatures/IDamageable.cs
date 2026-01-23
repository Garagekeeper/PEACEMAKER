namespace Resources.Script.Creatures
{
    public interface IDamageable
    {
        public float Hp { get; }
        bool IsDead { get;}
        public float DamageMultiplier { get; set; }
        public float SprayMultiplier { get; set; }

        public void OnDamage(float value, Creature attackBy, bool isCrit = false);
        public void OnDeath();
        public void Heal(float amount);

    }
}
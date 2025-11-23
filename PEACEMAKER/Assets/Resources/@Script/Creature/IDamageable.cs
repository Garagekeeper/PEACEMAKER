namespace Resources.Script.Creature
{
    public interface IDamageable
    {
        public float Hp { get;}
        bool IsDead { get;}

        public void OnDamage(float value, Resources.Script.Creature.Creature attackBy);
        public void OnDeath();
        public void Heal(float amount);

    }
}
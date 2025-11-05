using UnityEngine.Events;

namespace Resource.Script.Creature
{
    public interface IDamageable
    {
        public float Hp { get;}
        bool IsDead { get;}

        public void OnDamage(float value, Creature attackBy);
        public void OnDeath();
        public void Heal(float amount);

    }
}
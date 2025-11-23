using static Resources.Script.Defines;

namespace Resources.Script.Ammo
{
    public class AmmoType
    {
        public EAmmoType Type { get; private set; }
        public float BaseDamage { get; private set; }
        public float BasePenetration { get; private set; }
        public int BulletCountOnce { get;  private set; }

        //TODO 나중에 런타임에 값 저장
        public AmmoType(EAmmoType type, int baseDmg, float basePen, int bulletCountOnce = 1)
        {
            Type = type;
            BaseDamage = baseDmg;
            BasePenetration  = basePen;
            BulletCountOnce = bulletCountOnce;
        }
    }

    public class AmmoItem
    {
        public AmmoType ammo;
        public int Count { get; set; }
        public float DamageModifier { get; set; }
        public float PenetrationModifier { get; set; }

        public AmmoItem(AmmoType ammo, int count, float dmgMod, float PenMod)
        {
            this.ammo = ammo;
            this.Count = count;
            this.DamageModifier = dmgMod;
            this.PenetrationModifier = PenMod;
        }

        public float GetAmmoDmg()
        {
            return ammo.BaseDamage * DamageModifier;
        }
    }
}
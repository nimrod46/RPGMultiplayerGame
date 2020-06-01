using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;

namespace RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects
{
    internal class DemageBlast : IDamageInflicter
    {
        public float Damage { get; set; }
        public AnimatedObject.Direction Direction { get; set; }
        public Entity Attacker { get; set; }

        public DemageBlast(float damage)
        {
            Damage = damage;
        }

        public void Hit(Entity victim)
        {
            if (victim.IsDamageable)
            {
                victim.OnAttackedBy(Attacker, Damage);
            }
        }
    }
}
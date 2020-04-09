using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Weapons
{
    public class MeleeWeapon : Weapon
    {
        public MeleeWeapon(Point size, float damage, string name) : base(size, damage, name)
        {
        }

        internal override void Attack(Entity entity)
        {
            if(isInServer)
            {
                List<Entity> damagedEntities = GameManager.Instance.GetEntitiesHitBy(entity);
                if (damagedEntities.Count > 0)
                {
                    foreach (Entity damagedEntity in damagedEntities)
                    {
                        damagedEntity.OnAttackedBy(entity.SyncWeapon.SyncDamage);
                    }
                }
            }
        }
    }
}

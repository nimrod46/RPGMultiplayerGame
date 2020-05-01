using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects
{
    public class FreezingWeaponEffect : SpecielWeaponEffect
    {
        public FreezingWeaponEffect(Entity entity, IDamageInflicter damageInflicter) : base(entity, damageInflicter, 5, 1, false)
        {

        }

        public override void Activated()
        {
            entity.SetTinkColor(Color.DodgerBlue, 0.7f);
            if (entity.hasAuthority)
            {
                entity.SyncSpeed *= 0.5f;
            }
        }

        public override void Update()
        {
        }

        public override void End()
        {
            entity.ResetTint();
            if (entity.hasAuthority)
            {
                entity.SyncSpeed /= 0.5f;
            }
        }
    }
}

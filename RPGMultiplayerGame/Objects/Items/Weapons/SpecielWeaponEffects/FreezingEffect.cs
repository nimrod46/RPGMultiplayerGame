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
        public const int LASTING_TIME = 5;
        public FreezingWeaponEffect(Entity entity, IDamageInflicter damageInflicter) : base(entity, damageInflicter, LASTING_TIME, 1)
        {
            ScheduledAction();
        }

        public override void OnActivated()
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

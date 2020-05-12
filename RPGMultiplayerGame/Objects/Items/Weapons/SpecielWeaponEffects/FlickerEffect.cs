using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects
{
    public class FlickerEffect : SpecielWeaponEffect
    {
        public FlickerEffect(Entity entity, IDamageInflicter damageInflicter) : base(entity, damageInflicter, 0.15, 6, false)
        {

        }

        public override void Activated()
        {
            entity.SyncIsVisible = false;
        }

        public override void Update()
        {
            entity.SyncIsVisible = !entity.SyncIsVisible;
        }

        public override void End()
        {
            entity.SyncIsVisible = true;
        }
    }
}

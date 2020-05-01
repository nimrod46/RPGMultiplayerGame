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
        public FlickerEffect(Entity entity, IDamageInflicter damageInflicter) : base(entity, damageInflicter, 0.1, 6, false)
        {

        }

        public override void Activated()
        {
            entity.IsVisible = false;
        }

        public override void Update()
        {
            entity.IsVisible = !entity.IsVisible;
        }

        public override void End()
        {
            entity.IsVisible = true;
        }
    }
}

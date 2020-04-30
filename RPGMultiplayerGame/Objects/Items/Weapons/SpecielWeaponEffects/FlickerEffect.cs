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
        public FlickerEffect(Entity entity) : base(entity, 0.3, 4, false)
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

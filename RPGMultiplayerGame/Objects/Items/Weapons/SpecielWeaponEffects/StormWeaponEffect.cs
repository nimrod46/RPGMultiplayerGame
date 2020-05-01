using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.VisualEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects
{
    public class StormWeaponEffect : WeaponEffectWithVisual
    {
        public const int LASTING_TIME = 2;

        public StormWeaponEffect(Entity entity, IDamageInflicter damageInflicter) : base(entity, damageInflicter, LASTING_TIME, 1, false, new StormVisualEffect())
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            entity.MoveByDistanceAndDir(5, damageInflicter.Direction);
        }

    }
}

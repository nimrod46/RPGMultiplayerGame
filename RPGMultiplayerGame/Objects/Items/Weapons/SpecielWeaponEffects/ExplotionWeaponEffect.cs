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
    public class ExplotionWeaponEffect : WeaponEffectWithVisual
    {
        public ExplotionWeaponEffect(Entity entity, IDamageInflicter damageInflicter) : base(entity, damageInflicter, 5, 1, true, new ExplotionVisualEffect())
        {
        }

        public override void Activated()
        {
            base.Activated();
            if (entity.isInServer)
            {
                List<Entity> entities = entity.GetCurrentEntitiesInRadius(25);
                foreach (var entity in entities)
                {
                    damageInflicter.Hit(entity);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (entity.isInServer)
            {
                if(visualEffect.IsLoopAnimationFinished())
                {
                    visualEffect.InvokeBroadcastMethodNetworkly(nameof(visualEffect.Destroy));
                }
            }
        }
    }
}

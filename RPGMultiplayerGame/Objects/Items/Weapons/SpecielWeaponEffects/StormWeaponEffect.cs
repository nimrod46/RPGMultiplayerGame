using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.VisualEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.LivingEntities.Entity;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;

namespace RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects
{
    public class StormWeaponEffect : SpecielWeaponEffect
    {
        private WindStormVisualEffect windStorm;

        public StormWeaponEffect(Entity entity, IDamageInflicter damageInflicter) : base(entity, damageInflicter, 2, 1, false)
        {
        }

        public override void Activated()
        {
            if (entity.isInServer)
            {
                entity.InvokeBroadcastMethodNetworkly(nameof(entity.SetCurrentEntityState), State.Idle, entity.SyncCurrentDirection);
                windStorm = new WindStormVisualEffect
                {
                    SyncParent = entity,
                };
                windStorm = (WindStormVisualEffect)ServerManager.Instance.SpawnVisualEffect(windStorm);
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            entity.MoveByDistanceAndDir(5, damageInflicter.Direction);
        }

        public override void Update()
        {
        }

        public override void End()
        {
            if (entity.isInServer)
            {
                windStorm.InvokeBroadcastMethodNetworkly(nameof(windStorm.Destroy));
            }
        }
    }
}

using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.VisualEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.Other.MovingObject;

namespace RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects
{
    public class StormWeaponEffect : WeaponEffectWithVisual<StormVisualEffect>
    {
        public const float LASTING_TIME = 1.5f;

        public StormWeaponEffect(Entity entity, IDamageInflicter damageInflicter) : base(entity, damageInflicter, LASTING_TIME, 1, false, new StormVisualEffect())
        {
        }

        public override void Activated()
        {
            base.Activated();
            if(entity.isInServer)
            {
                entity.InvokeBroadcastMethodNetworkly(nameof(entity.SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, State.Idle, entity.SyncCurrentDirection);
                entity.SyncIsAbleToMove = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            entity.MoveByDistanceAndDir(5, damageInflicter.Direction);
        }

        public override void End()
        {
            base.End();
            if (entity.isInServer)
            {
                entity.SyncIsAbleToMove = true;
            }
        }

    }
}

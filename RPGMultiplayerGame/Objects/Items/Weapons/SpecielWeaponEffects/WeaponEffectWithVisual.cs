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
    public abstract class WeaponEffectWithVisual : SpecielWeaponEffect
    {
        protected VisualEffect visualEffect;

        public WeaponEffectWithVisual(Entity entity, IDamageInflicter damageInflicter, double delaySec, int loopCount, bool allowMultiple, VisualEffect visualEffect) : base(entity, damageInflicter, delaySec, loopCount, allowMultiple)
        {
            this.visualEffect = visualEffect;
            visualEffect.GetType();
            visualEffect.SyncParent = entity;
        }

        public override void Activated()
        {
            if (entity.isInServer)
            {
                Console.WriteLine(visualEffect);
                visualEffect = ServerManager.Instance.SpawnVisualEffect(visualEffect);
            }

        }
        
        public override void Update()
        {
        }

        public override void End()
        {
            if (entity.isInServer)
            {
                visualEffect.InvokeBroadcastMethodNetworkly(nameof(visualEffect.Destroy));
            }
        }
    }
}

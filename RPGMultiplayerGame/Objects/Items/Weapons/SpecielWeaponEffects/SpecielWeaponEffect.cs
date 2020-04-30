using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects
{
    public abstract class SpecielWeaponEffect : ISpecielWeaponEffect
    {
        public bool IsDestroyed
        {
            get => isDestroyed; set
            {
                isDestroyed = value;
                if (isDestroyed)
                {
                    End();
                }
            }
        }
        public bool AllowMultiple { get; set; }

        protected readonly Entity entity;
        private readonly double delaySec;
        private readonly int loopCount;
        private int currentLoopCount;
        private double timeSinceLastActivationSec;
        private bool isDestroyed;

        public SpecielWeaponEffect(Entity entity, double delaySec, int loopCount, bool allowMultiple)
        {
            this.entity = entity;
            this.delaySec = delaySec;
            this.loopCount = loopCount;
            currentLoopCount = 0;
            timeSinceLastActivationSec = 0;
            IsDestroyed = false;
            AllowMultiple = allowMultiple;
            entity.ScheduledNewAction(this);
            Activated();
        }

        public abstract void Activated();

        public void Update(GameTime gameTime)
        {
            if (isDestroyed)
            {
                return;
            }
            timeSinceLastActivationSec += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastActivationSec >= delaySec)
            {
                timeSinceLastActivationSec = 0;
                Update();
                currentLoopCount++;
                if (currentLoopCount == loopCount)
                {
                    IsDestroyed = true;
                }
            }
        }

        public abstract void Update();

        public abstract void End();
    }
}

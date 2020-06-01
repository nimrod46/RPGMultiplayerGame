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
        public bool IsEnabled { get; set; }

        protected readonly Entity entity;
        protected readonly IDamageInflicter damageInflicter;
        protected readonly double delaySec;
        protected readonly int loopCount;
        protected int currentLoopCount;
        protected double timeSinceLastActivationSec;
        private bool isDestroyed;
        private bool hasBeenActivated;

        public SpecielWeaponEffect(Entity entity, IDamageInflicter damageInflicter, double delaySec, int loopCount)
        {
            this.entity = entity;
            this.damageInflicter = damageInflicter;
            this.delaySec = delaySec;
            this.loopCount = loopCount;
            currentLoopCount = 0;
            timeSinceLastActivationSec = 0;
            IsDestroyed = false;
            hasBeenActivated = false;
            GameManager.Instance.AddUpdateObject(this);
            IsEnabled = true;
        }

        public void ScheduledAction()
        {
            entity.ScheduledNewAction(this);
        }

        public void Activate()
        {
            if (!hasBeenActivated)
            {
                OnActivated();
                hasBeenActivated = true;
            }
        }

        public abstract void OnActivated();

        public virtual void Update(GameTime gameTime)
        {
            if (isDestroyed)
            {
                return;
            }

            Activate();

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

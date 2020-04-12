using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public abstract class Monster : Human
    {
        static readonly Random rand = new Random();
        protected double generatePointTimeDelay = 5f;
        protected double timeSinceLastGeneratePoint;

        public Monster(GameManager.EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, nameFont, true)
        {
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.AddMonster(this);
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            base.OnDestroyed(identity);
            GameManager.Instance.RemoveMonster(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!hasAuthority)
            {
                return;
            }

            if (IsLookingAtObject)
            {

                EquippedWeapon.UpdateWeaponLocation(this);
                if (GameManager.Instance.GetEntitiesHitBy(EquippedWeapon, this).Any())
                {
                    SetCurrentEntityState((int)State.Idle, SyncCurrentDirection);
                    Attack();
                }
                else
                {
                    SetCurrentEntityState((int)State.Moving, SyncCurrentDirection);
                }
            }
            else
            {
                if (!HavePathToFollow())
                {
                    timeSinceLastGeneratePoint += gameTime.ElapsedGameTime.TotalSeconds;
                    if (timeSinceLastGeneratePoint > generatePointTimeDelay)
                    {
                        timeSinceLastGeneratePoint = 0;
                        nextPoint = new Vector2(rand.Next(0, GameManager.Instance.GetMapSize().X), rand.Next(0, GameManager.Instance.GetMapSize().Y));
                    }
                }
            }
        }

        protected override void LookAtGameObject(GameObject gameObject)
        {
            IsLookingAtObject = true;
            if ((State)syncCurrentEntityState == State.Attacking)
            {
                return;
            }
            Vector2 heading = GetBaseCenter() - gameObject.GetBaseCenter();
            Direction direction = GetDirection(heading);
            if (direction != (Direction)SyncCurrentDirection || (State)syncCurrentEntityState != State.Moving)
            {
                SetCurrentEntityState((int)State.Moving, (int)direction);
            }
        }
    }
}

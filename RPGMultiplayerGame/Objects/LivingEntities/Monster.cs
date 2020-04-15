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
        private readonly DictionarySortedByValue<Entity, float> targetPlayers = new DictionarySortedByValue<Entity, float>();

        public Monster(GameManager.EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, nameFont, true)
        {
            minDistanceForObjectInteraction = 40;
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

            List<Player> currentInteractingPlayers = GetCurrentPlayersInRadius();

            foreach (var player in currentInteractingPlayers)
            {
                if (!targetPlayers.ContainsKey(player))
                {
                    targetPlayers.Add(player, 0);
                }
            }

            foreach (var playerAndDamage in targetPlayers.ToList().Where(pl => !currentInteractingPlayers.Contains(pl.Key)))
            {
                if (playerAndDamage.Value == 0 || playerAndDamage.Key.IsDestroyed)
                {
                    targetPlayers.Remove(playerAndDamage);
                }
                InvokeBroadcastMethodNetworkly(nameof(StopLookingAtGameObject), playerAndDamage.Key);
            }

            if (targetPlayers.GetMaxElement().HasValue)
            {
                InvokeBroadcastMethodNetworkly(nameof(LookAtGameObject), targetPlayers.GetMaxElement().Value.Key);
            }

            if (IsLookingAtObject)
            {
                EquippedWeapon.UpdateWeaponLocation(this);
                if (GameManager.Instance.GetEntitiesHitBy(EquippedWeapon, this).Any())
                {
                    InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), (object)(int)State.Idle, SyncCurrentDirection);
                    Attack();
                }
                else
                {
                    InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), (object)(int)State.Moving, SyncCurrentDirection);
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

            base.LookAtGameObject(gameObject);
        }

        public override void OnAttackedBy(Entity attacker, float damage)
        {
            base.OnAttackedBy(attacker, damage);
            if (isInServer)
            {
                if (targetPlayers.TryGetValue(attacker, out float lastDamage))
                {
                    targetPlayers.Remove(attacker);
                    targetPlayers.Add(attacker, lastDamage + damage);
                }
                else
                {
                    targetPlayers.Add(attacker, damage);
                }
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public abstract class Monster : Human
    {
        static readonly Random rand = new Random();
        protected double generatePointTimeDelay = 5f;
        protected double timeSinceLastGeneratePoint;
        private readonly DictionarySortedByValue<Entity, float> targetPlayers = new DictionarySortedByValue<Entity, float>();

        public Monster(GraphicManager.EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, nameFont, true, Color.DarkRed)
        {
            minDistanceForObjectInteraction = 60;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!hasAuthority || isDead)
            {
                return;
            }

            List<Player> currentInteractingPlayers = GetCurrentPlayersInRadius();
            lock (targetPlayers)
            {
                foreach (var player in currentInteractingPlayers)
                {
                    if (!targetPlayers.ContainsKey(player))
                    {
                        targetPlayers.Add(player, 0);
                    }
                }

                foreach (var playerAndDamage in targetPlayers.ToList().Where(pl => !currentInteractingPlayers.Contains(pl.Key)))
                {
                    if (playerAndDamage.Value == 0 || playerAndDamage.Key.IsDestroyed || playerAndDamage.Key.SyncIsDead)
                    {
                        targetPlayers.Remove(playerAndDamage);
                    }
                    InvokeBroadcastMethodNetworkly(nameof(StopLookingAtGameObject), playerAndDamage.Key);
                }

                if (targetPlayers.GetMaxElement().HasValue)
                {
                    EquippedWeapon.UpdateWeaponLocation(this);
                    if (EquippedWeapon is MeleeWeapon meleeWeapon)
                    {
                        if (GameManager.Instance.GetEntitiesHitBy(meleeWeapon, this).Any(e => e is Player player && player == targetPlayers.GetMaxElement().Value.Key))
                        {
                            InvokeBroadcastMethodNetworkly(nameof(LookAtGameObject), targetPlayers.GetMaxElement().Value.Key, (int)State.Idle);
                            Attack();
                        }
                        else
                        {
                            InvokeBroadcastMethodNetworkly(nameof(LookAtGameObject), targetPlayers.GetMaxElement().Value.Key, (int)State.Moving);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
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
                            SyncNextPoint.MoveTo(new Vector2(rand.Next(0, GameManager.Instance.GeMapSize().X), rand.Next(0, GameManager.Instance.GeMapSize().Y)));
                        }
                    }
                }
            }
        }

        public override void Kill(Entity attacker)
        {
            base.Kill(attacker);
            if (isInServer)
            {
                attacker.OnEnitytKillEvent?.Invoke(this);
                new Thread(new ThreadStart(() => 
                {
                    Thread.Sleep(5000);
                    //InvokeBroadcastMethodNetworkly(nameof(Destroy)); 
                    Respawn(SyncX, SyncY);
                })).Start();
            }
        }

        public override void Respawn(float x, float y)
        {
            targetPlayers.Clear();
            base.Respawn(x, y);
        }

        public override void OnAttackedBy(Entity attacker, float damage)
        {
            base.OnAttackedBy(attacker, damage);
            if (isInServer)
            {
                lock (targetPlayers)
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
}

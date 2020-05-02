using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public abstract class PathEntity : Entity
    {

        public struct Waypoint
        {
            public Point Point { get; set; }
            public float Time { get; set; }

            public Waypoint(Point point, float time)
            {
                Point = point;
                Time = time;
            }
        }

        public bool IsLookingAtObject { get; set; }
        protected float minDistanceForObjectInteraction;
        private readonly List<Waypoint> path = new List<Waypoint>();
        private double currentTime = 0;
        private double currentPointTime = 0;
        private int nextWaypointIndex = 0;
        protected Vector2 nextPoint = Vector2.Zero;
        private int unit;


        public PathEntity(EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, bool damageable) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, damageable)
        {
            IsLookingAtObject = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!isServerAuthority || !isInServer || isDead)
            {
                return;
            }

            if (!IsLookingAtObject)
            {
                if (nextPoint == Vector2.Zero)
                {
                    return;
                }

                if (Vector2.Distance(new Vector2(SyncX, SyncY), nextPoint) <= 2f) //next point
                {
                    currentTime += gameTime.ElapsedGameTime.TotalSeconds;
                    if (currentPointTime != 0 && currentTime < currentPointTime)
                    {
                        if (GetCurrentEnitytState<State>() == State.Moving)
                        {
                            InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), false, (object)(int)State.Idle, SyncCurrentDirection);
                        }
                        return;
                    }

                    if (path.Count == nextWaypointIndex + 1)
                    {
                        unit = -1;
                    }
                    else if (nextWaypointIndex == 0)
                    {
                        unit = 1;
                    }

                    nextWaypointIndex += unit;
                    if (HavePathToFollow())
                    {
                        currentPointTime = path[nextWaypointIndex].Time;
                        nextPoint = path[nextWaypointIndex].Point.ToVector2();
                    }
                    currentTime = 0;
                }
                InvokeBroadcastMethodNetworkly(nameof(MoveToPoint), nextPoint.X, nextPoint.Y);
            }
        }

        protected List<Player> GetCurrentPlayersInRadius()
        {
            return GetCurrentPlayersInRadius(minDistanceForObjectInteraction);
        }

        protected List<Player> GetCurrentPlayersInRadius(float minDistance)
        {
            List<Player> currentInteractingPlayers = new List<Player>();
            for (int i = 0; i < ServerManager.Instance.players.Count; i++)
            {
                Player player = ServerManager.Instance.players[i];
                if (!player.SyncIsDead && IsObjectInInteractingRadius(player, minDistance))
                {
                    currentInteractingPlayers.Add(player);
                }
            }
            return currentInteractingPlayers;
        }

        protected bool IsObjectInInteractingRadius(GameObject gameObject)
        {
            return IsObjectInInteractingRadius(gameObject, minDistanceForObjectInteraction);
        }

        protected virtual void LookAtGameObject(GameObject gameObject, int entityState)
        {
            IsLookingAtObject = true;
            Vector2 heading = GetBaseCenter() - gameObject.GetBaseCenter();
            Direction direction = GetDirection(heading);
            if (direction != SyncCurrentDirection || syncCurrentEntityState != entityState)
            {
                SetCurrentEntityState(entityState, direction);
            }
        }

        protected virtual void StopLookingAtGameObject(GameObject gameObject)
        {
            IsLookingAtObject = false;
        }

        protected bool HavePathToFollow()
        {
            return path.Any();
        }

        protected void MoveToPoint(float x, float y)
        {
            Vector2 point = new Vector2(x, y);
            if (Vector2.Distance(new Vector2(SyncX, SyncY), point) <= 2f)
            {
                SetCurrentEntityState((int)State.Idle, SyncCurrentDirection);
                return;
            }

            Vector2 heading = new Vector2(SyncX, SyncY) - point;
            Direction direction = GetDirection(heading);
            if (GetCurrentEnitytState<State>() != State.Moving || direction != SyncCurrentDirection)
            {
                SetCurrentEntityState((int)State.Moving, direction);
            }
        }

        protected Direction GetDirection(Vector2 heading)
        {
            Direction direction;
            if (Math.Abs(heading.X) > Math.Abs(heading.Y))
            {
                if (heading.X > 0)
                {
                    direction = Direction.Left;
                }
                else
                {
                    direction = Direction.Right;
                }
            }
            else
            {
                if (heading.Y > 0)
                {
                    direction = Direction.Up;
                }
                else
                {
                    direction = Direction.Down;
                }
            }
            return direction;
        }

        public void AddWaypoint(Waypoint waypoint)
        {
            if (!path.Contains(waypoint))
            {
                path.Add(waypoint);
                nextPoint = path[0].Point.ToVector2();
            }
        }
    }
}

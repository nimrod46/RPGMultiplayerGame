using Map;
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
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public abstract class PathEntity : Entity
    {
        private const float MIN_DISTANCE_FOR_PLAYER_INTERACTION = 40;

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

        public bool IsLookingAtPlayer { get; set; }
        private readonly List<Waypoint> path = new List<Waypoint>();
        private double currentTime = 0;
        private double currentPointTime = 0;
        private int nextWaypointIndex = 0;
        protected Vector2 nextPoint = Vector2.Zero;
        private int unit;

        public PathEntity(EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth)
        {
            IsLookingAtPlayer = false;
        }
       
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!hasAuthority || !isInServer)
            {
                return;
            }

            float minDistance = MIN_DISTANCE_FOR_PLAYER_INTERACTION;
            Player closestPlayer = null;
            for (int i = 0; i < ServerManager.Instance.players.Count; i++)
            {
                Player player = ServerManager.Instance.players[i];
                float distance = Vector2.Distance(player.GetBaseCenter(), GetBaseCenter());

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPlayer = player;
                }
            }

            if (closestPlayer != null)
            {
                LookAtGameObject(closestPlayer);
            }
            else
            {
                if (IsLookingAtPlayer)
                {
                   
                    StopLookingAtGameObject();
                }

                if (nextPoint == Vector2.Zero)
                {
                    return;
                }
                if (Vector2.Distance(new Vector2(SyncX, SyncY), nextPoint) <= 2f || !(GetCurrentEnitytState<State>() == State.Moving)) //next point
                {
                    currentTime += gameTime.ElapsedGameTime.TotalSeconds;
                    if (currentPointTime != 0 && currentTime < currentPointTime)
                    {
                        if (GetCurrentEnitytState<State>() == State.Moving)
                        {
                            SetCurrentEntityState((int)State.Idle, (int)syncCurrentDirection);
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
                MoveToPoint(nextPoint);
            }
        }

        protected virtual void StopLookingAtGameObject()
        {
            InvokeBroadcastMethodNetworkly(nameof(StopLookingAtGameObject));
            IsLookingAtPlayer = false;
            if (nextPoint != Vector2.Zero)
            {
                MoveToPoint(nextPoint);
            }
        }

        protected bool HavePathToFollow()
        {
            return path.Any();
        }

        private void MoveToPoint(Vector2 point)
        {
            Vector2 heading = new Vector2(SyncX, SyncY) - point;
            Direction direction = GetDirection(heading);
            if (!(GetCurrentEnitytState<State>() == State.Moving) || (int) direction != syncCurrentDirection)
            {
                SetCurrentEntityState((int)State.Moving, (int)direction);
            }
        }

        protected virtual void LookAtGameObject(GameObject gameObject)
        {
            IsLookingAtPlayer = true;
            Vector2 heading = GetBaseCenter() - gameObject.GetBaseCenter();
            Direction direction = GetDirection(heading);
            if (direction != (Direction)syncCurrentDirection || (State)syncCurrentEntityState != State.Idle)
            {
                SetCurrentEntityState((int)State.Idle, (int)direction);
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
                nextPoint = waypoint.Point.ToVector2();
            }
        }
    }
}

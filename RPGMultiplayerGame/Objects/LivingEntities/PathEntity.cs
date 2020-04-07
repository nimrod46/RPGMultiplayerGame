using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
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

        public bool LookingAtPlayer { get; set; }
        private readonly List<Waypoint> path = new List<Waypoint>();
        private double currentTime = 0;
        private double currentPointTime = 0;
        private int nextWaypointIndex = 0;
        private Vector2 nextPoint = Vector2.Zero;
        private int unit;

        public PathEntity(EntityID entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth)
        {
            LookingAtPlayer = false;
        }
       
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (nextPoint == Vector2.Zero)
            {
                return;
            }

            if (!hasAuthority)
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
                if (LookingAtPlayer)
                {
                   
                    StopLookingAtGameObject();
                }

                if (Vector2.Distance(new Vector2(SyncX, SyncY), nextPoint) <= 2f || !(GetCurrentEnitytState() == EntityState.Moving)) //next point
                {
                    currentTime += gameTime.ElapsedGameTime.TotalSeconds;
                    if (currentPointTime != 0 && currentTime < currentPointTime)
                    {
                        if (GetCurrentEnitytState() == EntityState.Moving)
                        {
                            SetCurrentEntityState((int)EntityState.Idle, (int)syncCurrentDirection);
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
                    currentPointTime = path[nextWaypointIndex].Time;
                    nextPoint = path[nextWaypointIndex].Point.ToVector2();
                    currentTime = 0;
                    MoveToPoint(nextPoint);
                }
            }
        }

        protected virtual void StopLookingAtGameObject()
        {
            LookingAtPlayer = false;
            MoveToPoint(nextPoint);
        }

        private void MoveToPoint(Vector2 point)
        {
            Vector2 heading = new Vector2(SyncX, SyncY) - point;
            Direction direction = GetDirection(heading);
            SetCurrentEntityState((int)EntityState.Moving, (int)direction);
        }

        protected virtual void LookAtGameObject(GameObject gameObject)
        {
            LookingAtPlayer = true;
            Vector2 heading = GetBaseCenter() - gameObject.GetBaseCenter();
            Direction direction = GetDirection(heading);
            if (direction != (Direction)syncCurrentDirection || (EntityState)syncCurrentEntityState != EntityState.Idle)
            {
                SetCurrentEntityState((int)EntityState.Idle, (int)direction);
            }
        }

        private Direction GetDirection(Vector2 heading)
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

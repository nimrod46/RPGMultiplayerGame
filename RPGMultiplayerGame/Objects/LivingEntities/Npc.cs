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
   

    abstract class Npc : Human
    {
        private List<Waypoint> path = new List<Waypoint>();
        private double currentTime = 0;
        private double currentPointTime = 0;
        private int nextWaypointIndex = 0;
        private Vector2 nextPoint = Vector2.Zero;
        private int unit;
        public Npc(EntityID entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, nameFont)
        {
            syncDirection = (int)Direction.Idle;
            syncCurrentAnimationType = (int)Animation.IdleDown;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            speed *= 2f;
            layer -= 0.1f;
            syncIsMoving = false;
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
           
            if (Vector2.Distance(new Vector2(SyncX, SyncY), nextPoint) <= 2f || !syncIsMoving) //next point
            {
                currentTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (currentPointTime != 0 && currentTime < currentPointTime)
                {
                    if (syncIsMoving)
                    {
                        StopMoving();
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
                currentPointTime = path[nextWaypointIndex].SyncTime;
                nextPoint = new Vector2(path[nextWaypointIndex].SyncX, path[nextWaypointIndex].SyncY);
                currentTime = 0;
                Vector2 heading = new Vector2(SyncX, SyncY) - nextPoint;
                Direction direction = GetDirection(heading);
                StartMoving(direction);
            }
        }

        private Direction GetDirection(Vector2 heading)
        {
            Direction direction = Direction.Idle;
            heading.Normalize();
            if (heading.X > 0 && heading.X > 0.9)
            {
                direction = Direction.Left;
            }
            else if (heading.X < 0 && heading.X < -0.9)
            {
                direction = Direction.Right;
            }
            else if (heading.Y > 0 && heading.Y > 0.9)
            {
                direction = Direction.Up;
            }
            else if (heading.Y < 0 && heading.Y < -0.9)
            {
                direction = Direction.Down;
            }
            return direction;
        }

        public void AddWaypoint(Waypoint waypoint)
        {
            if (!path.Contains(waypoint))
            {
                path.Add(waypoint);
                nextPoint = new Vector2(path[0].SyncX, path[0].SyncY);
            }
        }
    }
}

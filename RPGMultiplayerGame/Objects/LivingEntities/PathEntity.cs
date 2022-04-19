using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Extention;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.MapObjects;
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
        protected readonly List<Waypoint> path = new List<Waypoint>();
        private double currentTime = 0;
        private double currentPointTime = 0;
        private int nextWaypointIndex = 0;
        private int unit;
        protected Vector2 nextPoint;


        public PathEntity(EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, bool damageable) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, damageable)
        {
            IsLookingAtObject = false;
            nextPoint = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(isDead)
            {
                return;
            }

            if (!IsLookingAtObject)
            {

                if (nextPoint == Vector2.Zero)
                {
                    return;
                }


                if (!isServerAuthority || !isInServer)
                {
                    return;
                }
                MoveToPoint(nextPoint.X, nextPoint.Y);
                
                if (Vector2.Distance(new Vector2(SyncX, SyncY), nextPoint) <= 2f) //next point
                {
                    if (HavePathToFollow())
                    {
                        currentTime += gameTime.ElapsedGameTime.TotalSeconds;
                        if (currentPointTime != 0 && currentTime < currentPointTime)
                        {
                            if (GetCurrentEnitytState<State>() == State.Moving)
                            {
                                InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, (int)State.Idle, SyncCurrentDirection);
                            }
                            return;
                        }

                        currentPointTime = path[nextWaypointIndex].Time;
                        nextPoint = path[nextWaypointIndex].Point.ToVector2();
                        NextPointChanged();
                        if (path.Count <= nextWaypointIndex + 1)
                        {
                            unit = -1;
                            FinishedPath();
                        }
                        else if (nextWaypointIndex == 0)
                        {
                            unit = 1;
                        }
                        nextWaypointIndex += unit;
                        currentTime = 0;
                    }
                }
            }
        }

        protected virtual void NextPointChanged()
        {
        }

        protected virtual void FinishedPath()
        {
        }

        public override void Kill(Entity attacker)
        {
            base.Kill(attacker);
            IsLookingAtObject = false;
        }

        protected List<Player> GetCurrentPlayersInRadiusAndSight()
        {
            return GetCurrentPlayersInRadiusAndSight(minDistanceForObjectInteraction);
        }

        protected List<Player> GetCurrentPlayersInRadiusAndSight(float minDistance)
        {
            List<Player> currentInteractingPlayers = new List<Player>();
            for (int i = 0; i < ServerManager.Instance.players.Count; i++)
            {
                Player player = ServerManager.Instance.players[i];
                if (IsPlayerInRadiusAndSight(player, minDistance))
                {
                    currentInteractingPlayers.Add(player);
                }
            }
            return currentInteractingPlayers;
        }

        protected bool IsPlayerInRadiusAndSight(Player player, float minDistance)
        {
            return !player.SyncIsDead && IsObjectInInteractingRadius(player, minDistance) && IsObjectInSight(player);
        }

        protected bool IsObjectInInteractingRadius(GameObject gameObject)
        {
            return IsObjectInInteractingRadius(gameObject, minDistanceForObjectInteraction);
        }

        protected bool IsObjectInSight(GameObject gameObject)
        {
            return GameManager.Instance.Map.HasClearSight(Location, gameObject.Location);
        }

        protected virtual void LookAtGameObject(GameObject gameObject, int entityState)
        {
            IsLookingAtObject = true;
            Direction direction = GetDirectionToGameObject(gameObject);
            if (direction != SyncCurrentDirection || SyncCurrentEntityState != entityState)
            {
                InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, entityState, direction);
            }
        }

        protected virtual void StopLookingAtGameObject(GameObject gameObject)
        {
            IsLookingAtObject = false;
        }

        protected void ClearPath()
        {
            path.Clear();
            nextWaypointIndex = 0;
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
                InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, (int)State.Idle, SyncCurrentDirection);
                return;
            }

            Vector2 heading = new Vector2(SyncX, SyncY) - point;
            Direction direction = Operations.GetDirection(heading);
            if (GetCurrentEnitytState<State>() != State.Moving || direction != SyncCurrentDirection)
            {
                InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, (int)State.Moving, direction);
            }
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

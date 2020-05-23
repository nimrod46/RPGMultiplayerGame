using Map;
using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Extention;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;
using System;
using System.Collections.Generic;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Objects.Other
{
    public class MovingObject : AnimatedObject
    {
        public enum State
        {
            Idle,
            Moving,
        }

        public bool SyncIsAbleToMove
        {
            get => isAbleToMove;
            set
            {
                isAbleToMove = value;
                InvokeSyncVarNetworkly(nameof(SyncIsAbleToMove), isAbleToMove);
            }
        }

        public int SyncCurrentEntityState { get; set; }

        public Vector2 CollisionSize { get; set; }

        public PositionType CollisionSizeType { get; set; }

        protected int collisionOffsetX;
        protected int collisionOffsetY;
        private bool isAbleToMove;

        public MovingObject(Dictionary<int, List<GameTexture>> animationsByType, int collisionOffsetX, int collisionOffsetY) : base(animationsByType)
        {
            this.collisionOffsetX = collisionOffsetX;
            this.collisionOffsetY = collisionOffsetY;
            Layer = GraphicManager.MOVING_OJECT_LAYER;
            SyncIsAbleToMove = true;
            CollisionSize = Vector2.Zero;
            CollisionSizeType = PositionType.None;
        }

        public virtual void SetCurrentEntityState(int entityState, Direction direction)
        {
            SyncCurrentEntityState = entityState;
            switch ((State)SyncCurrentEntityState)
            {
                case State.Idle:
                    IdleAtDir(direction);
                    break;
                case State.Moving:
                    MoveAtDir(direction);
                    break;
            }
            Location = new Vector2(SyncX, SyncY);
        }

        protected virtual void IdleAtDir(Direction direction)
        {
            AnimationAtDir(direction, 4, true);
        }

        protected virtual void MoveAtDir(Direction direction)
        {
            AnimationAtDir(direction, 0, true);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GetCurrentEnitytState<State>() == State.Moving && SyncIsAbleToMove)
            {
                float movment = SyncSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                MoveByDistanceAndDir(movment, SyncCurrentDirection);
            }
        }

        public void MoveByDistanceAndDir(float speed, Direction direction)
        {
            Vector2 newLocation = Location;
            switch (direction)
            {
                case Direction.Up:
                    newLocation.Y -= speed;
                    break;
                case Direction.Down:
                    newLocation.Y += speed;
                    break;
                case Direction.Left:
                    newLocation.X -= speed;
                    break;
                case Direction.Right:
                    newLocation.X += speed;
                    break;
            }
            Rectangle newLocationRect;

            newLocationRect = GetCollisionRectAt(newLocation);
            if (!GameManager.Instance.Map.TryGetHighBlockAt(newLocationRect, out Block block))
            {
                Location = newLocation;
                if (hasAuthority)
                {
                    SyncX = newLocation.X;
                    SyncY = newLocation.Y;
                }
            }
            else
            {
                OnCollidingWithBlock(block);
            }
        }

        protected virtual void OnCollidingWithBlock(Block block)
        {
        }

        public T GetCurrentEnitytState<T>() where T : Enum
        {
            return (T)(object)SyncCurrentEntityState;
        }

        public Rectangle GetCollisionRect()
        {
            return GetCollisionRectAt(new Vector2(SyncX, SyncY));
        }

        public Rectangle GetCollisionRectAt(Vector2 location)
        {
            if (CollisionSizeType == PositionType.None)
            {
                CollisionSize = BaseSize.ToVector2();
            }
            else
            {
                location = Operations.GetPositionByTopLeftPosition(CollisionSizeType, location, Size.ToVector2());
                location = Operations.GetTopLeftPositionByPorsitionType(CollisionSizeType, location, CollisionSize);
            }
            return new Rectangle((int)location.X + collisionOffsetX, (int)location.Y + collisionOffsetY, (int)CollisionSize.X - collisionOffsetX, (int)CollisionSize.Y - collisionOffsetY);
        }
    }
}

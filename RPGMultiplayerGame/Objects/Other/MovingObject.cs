using Map;
using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.MathExtention;
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
                float movment = SyncSpeed * (float) gameTime.ElapsedGameTime.TotalMilliseconds;
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
            MapObjectLib block = null;
            Rectangle newLocationRect;
            
            newLocationRect = GetCollisionRect(newLocation, CollisionSize);
            System.Drawing.Rectangle rectt;
            rectt = new System.Drawing.Rectangle(newLocationRect.X, newLocationRect.Y, newLocationRect.Width, newLocationRect.Height);
            for (int i = 0; i < GameManager.Instance.map.GraphicObjects.Count; i++)
            {
                if (GameManager.Instance.map.GraphicObjects[i] is BlockLib && GameManager.Instance.map.GraphicObjects[i].Layer > 0 && GameManager.Instance.map.GraphicObjects[i].Rectangle.IntersectsWith(rectt))
                {
                    block = GameManager.Instance.map.GraphicObjects[i];
                    break;
                }
            }

            if (block == null)
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
                OnCollidingWithBlock(block as BlockLib);
            }
        }

        protected virtual void OnCollidingWithBlock(BlockLib block)
        {
        }

        public T GetCurrentEnitytState<T>() where T : Enum
        {
            return (T)(object)SyncCurrentEntityState;
        }

        private Rectangle GetCollisionRect(Vector2 location, Vector2 size)
        {
            if (CollisionSizeType == PositionType.None)
            {
                CollisionSize = BaseSize.ToVector2();
            }
            else
            {
                location = Operations.GetPositionByTopLeftPosition(CollisionSizeType, location, BaseSize.ToVector2());
                location = Operations.GetTopLeftPositionByPorsitionType(PositionType.Centered, location, CollisionSize);
            }
            return new Rectangle((int)location.X + collisionOffsetX, (int)location.Y + collisionOffsetY, (int) size.X - collisionOffsetX, (int) size.Y - collisionOffsetY);
        }
    }
}

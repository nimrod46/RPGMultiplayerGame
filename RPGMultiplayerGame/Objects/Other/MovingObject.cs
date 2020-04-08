using Map;
using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Other
{
    public class MovingObject : AnimatedObject
    {
        public enum State
        {
            Idle,
            Moving,
        }

        protected int syncCurrentEntityState;
        protected int collisionOffsetX;
        protected int collisionOffsetY;

        public MovingObject(Dictionary<int, List<GameTexture>> animationsByType, int collisionOffsetX, int collisionOffsetY) : base(animationsByType)
        {
            this.collisionOffsetX = collisionOffsetX;
            this.collisionOffsetY = collisionOffsetY;
        }

        public virtual void SetCurrentEntityState(int entityState, int direction)
        {
            InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), entityState, direction );
            syncCurrentEntityState = entityState;
            switch ((State)syncCurrentEntityState)
            {
                case State.Idle:
                    IdleAtDir((Direction)direction);
                    break;
                case State.Moving:
                    MoveAtDir((Direction)direction);
                    break;
            }
            Location = new Vector2(SyncX, SyncY);
        }

        protected void IdleAtDir(Direction direction)
        {
            AnimationAtDir(direction, 4, true);
        }

        protected void MoveAtDir(Direction direction)
        {
            AnimationAtDir(direction, 0, true);
        }

       

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GetCurrentEnitytState<State>() == State.Moving)
            {
                MapObjectLib block = null;
                Rectangle newLocationRect;
                System.Drawing.Rectangle rectt;
                double movment = speed * gameTime.ElapsedGameTime.TotalMilliseconds;
                Vector2 newLocation = Location;
                switch ((Direction)syncCurrentDirection)
                {
                    case Direction.Up:
                        newLocation.Y -= (float)movment;
                        break;
                    case Direction.Down:
                        newLocation.Y += (float)movment;
                        break;
                    case Direction.Left:
                        newLocation.X -= (float)movment;
                        break;
                    case Direction.Right:
                        newLocation.X += (float)movment;
                        break;
                }
                newLocationRect = GetCollisionRect(newLocation.X, newLocation.Y, Size.X, Size.Y);
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
                    if (hasAuthority)
                    {
                        SyncX = newLocation.X;
                        SyncY = newLocation.Y;
                    }
                    Location = newLocation;
                }
            }
        }

        public T GetCurrentEnitytState<T>() where T : Enum
        {
            return (T) (object) syncCurrentEntityState;
        }

        private Rectangle GetCollisionRect(float x, float y, int width, int height)
        {
            return new Rectangle((int)x + collisionOffsetX, (int)y + collisionOffsetY, width - collisionOffsetX, height - collisionOffsetY);
        }

        protected override void InitAnimationsList()
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Ui
{
    public abstract class UiComponent : IGameDrawable
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }

        public Vector2 Size
        {
            get => size; set
            {
                size = value;
                UpdatePosition();
            }
        }

        public Vector2 Origin
        {
            get => origin; set
            {
                origin = value;
                UpdatePosition();
            }
        }

        public PositionType OriginType { get { return originType; }
            set
            {
                originType = value;
                UpdatePosition();
            }
        }
        private PositionType originType;
        private Vector2 origin;
        private Vector2 size;

        public enum PositionType
        {
            Centered,
            ButtomLeft,
            ButtomCentered,
            TopLeft,
            TopRight
        }

        public UiComponent(Vector2 origin, PositionType originType, Texture2D texture)
        {
            Origin = origin;
            OriginType = originType;       
            Texture = texture;
            Size = texture.Bounds.Size.ToVector2();
        }

        public UiComponent(Vector2 origin, PositionType originType)
        {
            Origin = origin;
            OriginType = originType;
        }

        public UiComponent()
        {
            Origin = Vector2.Zero;
            Size = Vector2.Zero;
        }

        protected void UpdatePosition()
        {
            switch (OriginType)
            {
                case PositionType.Centered:
                    Position = Origin - Size / 2;
                    break;
                case PositionType.ButtomLeft:
                    Position = Origin - new Vector2(0, Size.Y);
                    break;
                case PositionType.ButtomCentered:
                    Position = Origin - new Vector2(Size.X / 2, Size.Y);
                    break;
                case PositionType.TopLeft:
                    Position = Origin;// - new Vector2(Size.X, -Size.Y);
                    break;
                case PositionType.TopRight:
                    Position = Origin - new Vector2(Size.X, 0);
                    break;
            }
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            sprite.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, GameManager.GUI_LAYER);
        }
    }
}

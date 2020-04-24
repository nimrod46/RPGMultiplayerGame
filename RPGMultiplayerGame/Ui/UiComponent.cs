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
        public Func<Point, Vector2> OriginFunc
        {
            get => originFunc; set
            {
                originFunc = value;
                Resize();
                UpdatePosition();
            }
        }

        public Vector2 Position { get; set; }

        public Vector2 DrawPosition { get; set; }

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

        public PositionType OriginType
        {
            get { return originType; }
            set
            {
                originType = value;
                UpdatePosition();
            }
        }

        public virtual bool IsVisible { get => isVisible; set => isVisible = value; }

        public float Layer { get; set; }

        public float Scale { get; set; }

        public UiComponent Parent;

        protected PositionType originType;
        protected Vector2 origin;
        protected Vector2 size;
        private Func<Point, Vector2> originFunc;
        protected bool isVisible;

        public enum PositionType
        {
            Centered,
            ButtomLeft,
            ButtomCentered,
            TopLeft,
            TopRight
        }



        public UiComponent(Func<Point, Vector2> originFunc, PositionType originType, bool defaultVisibility, float layer)
        {
            this.OriginFunc = originFunc;
            Origin = this.OriginFunc.Invoke(GameManager.Instance.GetScreenSize());
            OriginType = originType;
            isVisible = defaultVisibility;
            Layer = layer;
            Scale = 1;
            GameManager.Instance.AddUiComponent(this);
        }

        public UiComponent()
        {
            Origin = Vector2.Zero;
            Size = Vector2.Zero;
            OriginType = PositionType.TopLeft;
            Layer = 0;
            Scale = 1;
            GameManager.Instance.AddUiComponent(this);
        }

        protected virtual void UpdatePosition()
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
                    Position = Origin;
                    break;
                case PositionType.TopRight:
                    Position = Origin - new Vector2(Size.X, 0);
                    break;
            }

            if (Position.X + Size.X * Scale > GameManager.Instance.GetScreenSize().X)
            {
                Position += new Vector2(-Size.X * Scale, 0);
            }

            if (Position.Y + Size.Y * Scale > GameManager.Instance.GetScreenSize().Y)
            {
                Position += new Vector2(0, -Size.Y * Scale);
            }

            DrawPosition = Position + (Parent != null ? Parent.Position : Vector2.Zero);
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            if (IsVisible)
            {
                Origin = OriginFunc.Invoke(GameManager.Instance.GetScreenSize());
            }
        }

        public void Resize()
        {
            Origin = OriginFunc.Invoke(GameManager.Instance.GetScreenSize());
        }
    }
}

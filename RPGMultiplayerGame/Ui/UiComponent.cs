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

        public virtual bool IsVisible { get; set; }

        public float Layer { get; set; }

        protected PositionType originType;
        protected Vector2 origin;
        protected Vector2 size;
        private Func<Point, Vector2> originFunc;

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
            IsVisible = defaultVisibility;
            Layer = layer;
            GameManager.Instance.AddUiComponent(this);
        }

        public UiComponent()
        {
            Origin = Vector2.Zero;
            Size = Vector2.Zero;
            OriginType = PositionType.TopLeft;
            Layer = 0;
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
        }

        public abstract void Draw(SpriteBatch sprite);

        public void Resize()
        {
            Origin = OriginFunc.Invoke(GameManager.Instance.GetScreenSize());
        }
    }
}

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

        public virtual bool IsVisible { get; set; }

        public float Layer { get; set; }

        private readonly Func<Point, Vector2> originFunc;
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

        

        public UiComponent(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer)
        {
            originFunc = origin;
            Origin = originFunc.Invoke(GameManager.Instance.GetScreenSize());
            OriginType = originType;
            IsVisible = defaultVisibility;
            Layer = layer;
            GameManager.Instance.AddUiComponent(this);
        }

        public UiComponent()
        {
            Origin = Vector2.Zero;
            Size = Vector2.Zero;
            Layer = 0;
            GameManager.Instance.AddUiComponent(this);
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
            Origin = originFunc.Invoke(GameManager.Instance.GetScreenSize());
        }
    }
}

using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Extention;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;

namespace RPGMultiplayerGame.Graphics.Ui
{
    public abstract class UiComponent : IUiComponent, IGameUpdateable
    {
        [XmlIgnore]
        public Func<Point, Vector2> OriginFunc
        {
            get => originFunc; set
            {
                originFunc = value;
              //  Resize();
              //  UpdatePosition();
              Origin = originFunc.Invoke(UiManager.Instance.GetScreenSize());
            }
        }

        public Vector2 Position { get; private set; }

        protected Vector2 DrawPosition { get; private set; }

        public Vector2 Size
        {
            get => size * Scale;

            set
            {
                size = value;
                UpdatePosition();
            }
        }

        private Vector2 Origin
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

        public virtual bool IsVisible { get => Parent == null ? isVisible : Parent.IsVisible && isVisible;
            set => isVisible = value; }

        protected float Layer { get; }

        public float Scale { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsEnabled { get; set; }

        public UiComponent? Parent { get; set; }

        protected bool isVisible;
        private PositionType originType;
        private Vector2 origin;
        private Func<Point, Vector2> originFunc;
        private Vector2 size;

        public enum PositionType
        {
            None,
            Centered,
            ButtomLeft,
            ButtomCentered,
            TopLeft,
            TopRight,
            CenteredLeft,
            ButtomRight,
            TopCentered,
            CenteredRight
        }



        public UiComponent(Func<Point, Vector2> originFunc, PositionType originType, float layer)
        {
            Scale = 1;
            OriginFunc = originFunc;
            Origin = originFunc.Invoke(UiManager.Instance.GetScreenSize());
            OriginType = originType;
            isVisible = false;
            Layer = layer;
            UiManager.Instance.AddUiComponent(this);
            GameManager.Instance.AddUpdateObject(this);
        }

        public UiComponent()
        {
            Scale = 1;
            Origin = Vector2.Zero;
            OriginFunc = _ => Vector2.Zero;
            Size = Vector2.Zero;
            OriginType = PositionType.TopLeft;
            isVisible = false;
            Layer = 0;
            UiManager.Instance.AddUiComponent(this);
            GameManager.Instance.AddUpdateObject(this);
        }

        public virtual void UpdatePosition()
        {
            Position = Operations.GetTopLeftPositionByPorsitionType(OriginType, Origin, Size);

            if (Position.X + Size.X > UiManager.Instance.GetScreenSize().X)
            {
                Position += new Vector2(-Size.X, 0);
            }

            if (Position.Y + Size.Y > UiManager.Instance.GetScreenSize().Y)
            {
                Position += new Vector2(0, -Size.Y);
            }

            Position = (Parent != null ? Position * Parent.Scale : Position);
            DrawPosition = (Parent != null ? Parent.Position + Position : Position);
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            if (IsVisible)
            {
                Origin = OriginFunc.Invoke(UiManager.Instance.GetScreenSize());
            }
        }

        public virtual void Resize()
        {
            Origin = OriginFunc.Invoke(UiManager.Instance.GetScreenSize());
        }

        public void Delete()
        {
            UiManager.Instance.RemoveUiComponent(this);
            GameManager.Instance.RemoveUpdateObject(this);
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}

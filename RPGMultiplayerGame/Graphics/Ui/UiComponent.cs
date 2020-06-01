using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Graphics;
using RPGMultiplayerGame.Graphics.Ui;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Extention;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Xml.Serialization;

namespace RPGMultiplayerGame.Ui
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

        public Vector2 Position { get; set; }

        public Vector2 DrawPosition { get; set; }

        public Vector2 Size
        {
            get
            {
                return size * Scale;
            }

            set
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

        public virtual bool IsVisible { get { return Parent == null ? isVisible : Parent.IsVisible && isVisible; } set => isVisible = value; }

        public float Layer { get; set; }

        public float Scale { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsEnabled { get; set; }

        public UiComponent Parent;

        protected PositionType originType;
        protected Vector2 origin;
        private Func<Point, Vector2> originFunc;
        protected bool isVisible;
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
            this.OriginFunc = originFunc;
            Origin = this.OriginFunc.Invoke(UiManager.Instance.GetScreenSize());
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
            this.OriginFunc = (g) => Vector2.Zero;
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

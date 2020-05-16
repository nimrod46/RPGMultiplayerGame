using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.MathExtention;
using System;
using System.Xml.Serialization;

namespace RPGMultiplayerGame.Objects.Other
{
    public abstract class GraphicObject : UpdateableObject, IGameDrawable
    {
        public float Layer { get; set; }
        public float DefaultLayer { get; set; }
        public virtual bool SyncIsVisible
        {
            get => syncIsVisible; set
            {
                syncIsVisible = value;
                InvokeSyncVarNetworkly(nameof(SyncIsVisible), syncIsVisible);
            }
        }

        protected Texture2D Texture
        {
            get => texture; set
            {
                texture = value;
                Size = (Texture.Bounds.Size.ToVector2() * Scale).ToPoint();
                tintedTexture = texture;
            }
        }

        [XmlIgnore]
        public float Scale { get; protected set; }

        private Texture2D texture;
        protected Texture2D tintedTexture;
        protected Vector2 offset;
        protected Vector2 drawLocation;
        private Color tintColor;
        private float tintingAlpah;
        private bool syncIsVisible;

        public GraphicObject()
        {
            Layer = 1;
            Scale = 1;
            offset = Vector2.Zero;
            drawLocation = Vector2.Zero;
            SyncIsVisible = true;
            Layer -= 0.01f;
            tintColor = Color.White;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            if (!isInServer)
            {
                GraphicManager.Instance.AddGraphicObject(this);
            }
            DefaultLayer = Layer;
        }

        public void SetTinkColor(Color tintColor, float tintingAlpah)
        {
            this.tintColor = tintColor;
            this.tintingAlpah = tintingAlpah;
        }

        public void ResetTint()
        {
            tintColor = Color.White;
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            if (tintedTexture != null)
            {
                if (SyncIsVisible)
                {
                    sprite.Draw(Operations.TintTextureByColor(sprite.GraphicsDevice, texture, tintColor, tintingAlpah), Location + offset, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, Layer);
                }
            }
            else
            {
                Console.Error.WriteLine("Cannot draw object: " + Id + " " + GetType());
            }
        }

        public virtual Vector2 GetDrawCenter()
        {
            return Location + offset + Size.ToVector2() / 2;
        }

        public virtual Rectangle GetBoundingRectangle()
        {
            return new Rectangle((int)(SyncX + offset.X), (int)(SyncY + offset.Y), Size.X, Size.Y);
        }

        public bool IsIntersectingWith(GraphicObject graphicObject)
        {
            return GetBoundingRectangle().Intersects(graphicObject.GetBoundingRectangle());
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            GraphicManager.Instance.RemoveGraphicObject(this);
            base.OnDestroyed(identity);
        }
    }
}

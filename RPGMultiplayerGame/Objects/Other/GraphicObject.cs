using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using System;

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

        public Texture2D TintTextureByColor(GraphicsDevice graphicsDevice, Texture2D texture, Color color, float tintingAlpah)
        {
            if (color == Color.White)
            {
                return texture;
            }

            int pixelCount = Texture.Width * Texture.Height;
            Color[] pixels = new Color[pixelCount];
            texture.GetData(pixels);
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].A < 10)
                {
                    continue;
                }
                byte r = (byte)Math.Min(pixels[i].R * tintingAlpah + color.R * (1 - tintingAlpah), 255);
                byte g = (byte)Math.Min(pixels[i].G * tintingAlpah + color.G * (1 - tintingAlpah), 255);
                byte b = (byte)Math.Min(pixels[i].B * tintingAlpah + color.B * (1 - tintingAlpah), 255);
                pixels[i] = new Color(r, g, b, pixels[i].A);
            }
            Texture2D outTexture = new Texture2D(graphicsDevice, texture.Width, texture.Height, false, SurfaceFormat.Color);
            outTexture.SetData<Color>(pixels);
            return outTexture;
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
                    sprite.Draw(TintTextureByColor(sprite.GraphicsDevice, texture, tintColor, tintingAlpah), Location + offset, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, Layer);
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

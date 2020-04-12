using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.Other
{
    public abstract class GraphicObject : UpdateObject
    {
        public float Layer { get; set; }
        public float DefaultLayer { get; set; }

        protected Texture2D texture;
        protected Vector2 offset;
        protected Vector2 drawLocation;
        protected float scale;
        protected bool isVisible;


        public GraphicObject()
        {
            Layer = 1;
            scale = 1;
            offset = Vector2.Zero;
            drawLocation = Vector2.Zero;
            isVisible = true;
            Layer -= 0.01f;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.AddGraphicObject(this);
            DefaultLayer = Layer;
        }

        public virtual Rectangle GetBoundingRectangle()
        {
            return new Rectangle((int)(SyncX + offset.X), (int)(SyncY + offset.Y), Size.X, Size.Y);
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            if (texture != null)
            {
                if (isVisible)
                {
                    sprite.Draw(texture, Location + offset, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, Layer);
                }
            }
            else
            {
                Console.Error.WriteLine("Cannot draw object: " + Id + " " + GetType());
            }
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            GameManager.Instance.RemoveGraphicObject(this);
            base.OnDestroyed(identity);
        }
    }
}

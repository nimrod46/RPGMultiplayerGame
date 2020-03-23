using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects
{
    public abstract class GraphicObject : GameObject
    {
        public float Layer { get; set; }
        public float DefaultLayer { get; set; }

        protected Texture2D texture;
        protected Vector2 offset;
        protected Vector2 drawLocation;
        protected float scale;


        public GraphicObject()
        {
            Layer = 1;
            scale = 1;
            offset = Vector2.Zero;
            drawLocation = Vector2.Zero;
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
                sprite.Draw(texture, Location + offset, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, Layer);
            }
            else
            {
                Console.Error.WriteLine("Cannot draw object object: " + id + " " + GetType());
            }
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            GameManager.Instance.RemoveGraphicObject(this);
            base.OnDestroyed(identity);
        }
    }
}

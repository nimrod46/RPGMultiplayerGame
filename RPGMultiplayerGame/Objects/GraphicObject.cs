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
        
        protected Texture2D texture;
        protected Vector2 offset;
        protected Vector2 drawLocation;
        protected float layer;
        protected float scale;
        public GraphicObject()
        {
            layer = 1;
            scale = 1;
            offset = Vector2.Zero;
            drawLocation = Vector2.Zero;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.AddGraphicObject(this);
            controling = hasAuthority;
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            if (texture != null)
            {
                drawLocation = new Vector2(SyncX + offset.X, SyncY + offset.Y);
                sprite.Draw(texture, drawLocation, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);
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

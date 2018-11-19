using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Other;

namespace RPGMultiplayerGame.Networking
{
    public abstract class GraphicObject : NetworkIdentity
    {
        public Vector2 Location { get; set; }
        [SyncVar(networkInterface = NetworkInterface.UDP, hook = "OnXSet")]
        public float SyncX { get; set; }
        [SyncVar(networkInterface = NetworkInterface.UDP, hook = "OnYSet")]
        public float SyncY { get; set; }
        protected Texture2D texture;

        public override void OnNetworkInitialize()
        {
            Location = new Vector2(SyncX, SyncY);
        }

        public void OnXSet()
        {
            Location = new Vector2(SyncX, Location.Y);
        }

        public void OnYSet()
        {
            Location = new Vector2(Location.X, SyncY);
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            if (texture != null)
            {
                sprite.Draw(texture, Location, Color.White);
            }
            else
            {
                Console.Error.WriteLine("Cannot draw object object: " + id + " " + GetType());
            }
        }

        public abstract void Update(GameTime gameTime);
    }
}

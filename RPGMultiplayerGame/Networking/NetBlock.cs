using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Networking
{
    class NetBlock : NetworkIdentity
    {
        [SyncVar(hook = "OnTextureIndexSet")]
        public int SyncTextureIndex { get; set; }
        public Vector2 Location { get; set; }
        [SyncVar(hook = "OnXSet")]
        public float SyncX;
        [SyncVar(hook = "OnXSet")]
        public float SyncY;
        [SyncVar()]
        public int SyncLayer { get; set; }
        private Texture2D texture;

        public override void OnNetworkInitialize()
        {
            texture = MapManager.Instance.textures[SyncTextureIndex];
            Location = new Vector2(SyncX, SyncY);
            MapManager.Instance.blocks.Add(this);
        }

        public void OnXSet()
        {
            Location = new Vector2(SyncX, Location.Y);
        }

        public void OnYSet()
        {
            Location = new Vector2(Location.X, SyncY);
        }

        public void OnTextureIndexSet()
        {
            texture = MapManager.Instance.textures[SyncTextureIndex];
        }

        public void Draw(SpriteBatch sprite)
        {
            if (texture != null)
            {
                sprite.Draw(texture, Location, Color.White);
            }
        }
    }
}

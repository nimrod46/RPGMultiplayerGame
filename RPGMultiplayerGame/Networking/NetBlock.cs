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
        [SyncVar()]
        public int SyncTextureIndex { get; set; }
        public Vector2 Location { get; set; }
        [SyncVar(hook = "OnXSet")]
        public float SyncX;
        [SyncVar(hook = "OnXSet")]
        public float SyncY;
        [SyncVar()]
        public int SyncLayer { get; set; }
        private Texture2D texture;

        public override void OnNetworkInitialize(params object[] objects)
        {
            if (objects.Length > 0)
            {
                SyncTextureIndex = int.Parse(objects[0].ToString());
                SyncX = float.Parse(objects[1].ToString());
                SyncY =  float.Parse(objects[2].ToString());
                SyncLayer = int.Parse(objects[3].ToString());
                texture = MapManager.Instance.textures[SyncTextureIndex];
            }
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

        public void Draw(SpriteBatch sprite)
        {
            if (texture != null)
            {
                sprite.Draw(texture, Location, Color.White);
            }
        }
    }
}

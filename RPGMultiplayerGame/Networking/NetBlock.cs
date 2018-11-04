using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Networking
{
    class NetBlock : GameObject
    {
        [SyncVar(hook = "OnTextureIndexSet")]
        public int SyncTextureIndex { get; set; }
        [SyncVar()]
        public int SyncLayer { get; set; }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            texture = MapManager.Instance.textures[SyncTextureIndex];
            Location = new Vector2(SyncX, SyncY);
        }

        public void OnTextureIndexSet()
        {
            texture = MapManager.Instance.textures[SyncTextureIndex];
        }
    }
}

﻿using Map;
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
    public class NetBlock : MapObject
    {
        [SyncVar(hook = "OnTextureIndexSet")]
        public int SyncTextureIndex { get; set; }
        [SyncVar()]
        public int SyncLayer { get; set; }
        [SyncVar()]
        public bool SyncHasUnder { get; set; }
        [SyncVar()]
        public bool SyncHasAbove { get; set; }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            texture = MapManager.Instance.textures[SyncTextureIndex];
            Location = new Vector2(SyncX, SyncY);
            if (!isInServer)
            {
                MapManager.Instance.map.AddBlockAt(new System.Drawing.Rectangle((int)SyncX, (int)SyncY, texture.Width, texture.Height), SyncTextureIndex, SyncLayer);
            }
            else
            {
                if (MapManager.Instance.map.spawn.Rectangle.Equals(new System.Drawing.Rectangle((int)SyncX, (int)SyncY, texture.Width, texture.Height)))
                {
                    MapManager.Instance.spawnPoint = this;
                    NetworkManager.Instance.UpdateSpawnLocation(MapManager.Instance.spawnPoint);
                }
            }
        }

        public void OnTextureIndexSet()
        {
            texture = MapManager.Instance.textures[SyncTextureIndex];
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}

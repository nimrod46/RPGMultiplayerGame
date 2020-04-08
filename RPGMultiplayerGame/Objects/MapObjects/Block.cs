using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGMultiplayerGame.Objects.MapObjects;

namespace RPGMultiplayerGame.Objects
{
    public class Block : MapObject
    {

        public int SyncTextureIndex
        {
            get => syncTextureIndex; set
            {
                syncTextureIndex = value;
                InvokeSyncVarNetworkly(nameof(SyncTextureIndex), value);
                OnTextureIndexSet();
            }
        }

        private int syncTextureIndex;


        public override void OnNetworkInitialize()
        {
            texture = GameManager.Instance.textures[SyncTextureIndex];
            base.OnNetworkInitialize();
            Init<BlockLib>();
        }

        public void OnTextureIndexSet()
        {
            texture = GameManager.Instance.textures[SyncTextureIndex];
        }

        protected override MapObjectLib CreateMapObject()
        {
            return new BlockLib(SyncTextureIndex, new System.Drawing.Rectangle((int)SyncX, (int)SyncY, texture.Width, texture.Height), SyncLayer);
        }
    }
}

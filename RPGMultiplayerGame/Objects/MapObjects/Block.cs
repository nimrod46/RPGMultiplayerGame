using Map;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.MapObjects;
using System.Collections.Generic;

namespace RPGMultiplayerGame.Objects
{
    public abstract class Block : MapObject
    {
        public int SyncTextureIndex
        {
            get => textureIndex; set
            {
                textureIndex = value;
                InvokeSyncVarNetworkly(nameof(SyncTextureIndex), value);
                OnTextureIndexSet();
            }
        }
        private int textureIndex;

        public Block(Dictionary<int, List<GameTexture>> animationsByType) : base(animationsByType)
        {
        }


        public override void OnNetworkInitialize()
        {
            //animationsByType = GraphicManager.Instance.Textures[SyncTextureIndex];
            base.OnNetworkInitialize();
            //Init<BlockLib>();
        }
        
        public abstract void OnTextureIndexSet();
    }
}

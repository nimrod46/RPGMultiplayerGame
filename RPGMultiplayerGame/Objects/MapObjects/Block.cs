using Map;
using RPGMultiplayerGame.Managers;
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
            texture = GraphicManager.Instance.Textures[SyncTextureIndex];
            base.OnNetworkInitialize();
            Init<BlockLib>();
        }

        public void OnTextureIndexSet()
        {
            texture = GraphicManager.Instance.Textures[SyncTextureIndex];
        }

        protected override MapObjectLib CreateMapObject()
        {
            return new BlockLib(SyncTextureIndex, new System.Drawing.Rectangle((int)SyncX, (int)SyncY, texture.Width, texture.Height), SyncLayer);
        }
    }
}

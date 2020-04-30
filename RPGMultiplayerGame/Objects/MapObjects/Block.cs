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
            Texture = GraphicManager.Instance.Textures[SyncTextureIndex];
            base.OnNetworkInitialize();
            Init<BlockLib>();
        }

        public void OnTextureIndexSet()
        {
            Texture = GraphicManager.Instance.Textures[SyncTextureIndex];
        }

        protected override MapObjectLib CreateMapObject()
        {
            return new BlockLib(SyncTextureIndex, new System.Drawing.Rectangle((int)SyncX, (int)SyncY, Texture.Width, Texture.Height), SyncLayer);
        }
    }
}

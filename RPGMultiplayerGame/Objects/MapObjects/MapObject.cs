using Map;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;

namespace RPGMultiplayerGame.Objects.MapObjects
{
    public abstract class MapObject : GraphicObject
    {
        private int syncLayer;
        private bool syncHasUnder;
        private bool syncHasAbove;

        public int SyncLayer
        {
            get => syncLayer; set
            {
                syncLayer = value;
                InvokeSyncVarNetworkly(nameof(SyncLayer), value);
            }
        }
        public bool SyncHasUnder
        {
            get => syncHasUnder; set
            {
                syncHasUnder = value;
                InvokeSyncVarNetworkly(nameof(SyncHasUnder), value);
            }
        }

        public bool SyncHasAbove
        {
            get => syncHasAbove; set
            {
                syncHasAbove = value;
                InvokeSyncVarNetworkly(nameof(SyncHasAbove), value);
            }
        }


        public override void OnNetworkInitialize()
        {
            Layer -= SyncLayer / 1000.0f;
            base.OnNetworkInitialize();
        }

        protected void Init<T>() where T : MapObjectLib
        {
            if (!isInServer)
            {
                MapObjectLib obj = CreateMapObject();
                GameManager.Instance.map.AddObjectAt(obj);
            }
        }

        protected abstract MapObjectLib CreateMapObject();
    }
}

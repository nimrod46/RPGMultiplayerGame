using Map;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;
using System.Collections.Generic;

namespace RPGMultiplayerGame.Objects.MapObjects
{
    public abstract class MapObject : AnimatedObject
    {
        private int syncLayer;
        private bool syncHasUnder;
        private bool syncHasAbove;

        public MapObject(Dictionary<int, List<GameTexture>> animationsByType) : base(animationsByType)
        {
        }

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

        public abstract bool Isblocking();


        public override void OnNetworkInitialize()
        {
            Layer -= SyncLayer / 1000.0f;
            GameManager.Instance.Map.AddBlock(this);
            base.OnNetworkInitialize();
        }
    }
}

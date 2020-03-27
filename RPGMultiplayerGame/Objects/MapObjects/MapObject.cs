using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map;
using Microsoft.Xna.Framework;
using Networking;

namespace RPGMultiplayerGame.Objects.MapObjects
{
    public abstract class MapObject : GraphicObject
    {
        [SyncVar]
        public int SyncLayer { get; set; }
        [SyncVar]
        public bool SyncHasUnder { get; set; }
        [SyncVar]
        public bool SyncHasAbove { get; set; }
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

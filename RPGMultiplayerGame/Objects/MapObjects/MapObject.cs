using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects
{
    public abstract class MapObject : GraphicObject
    {
        public override void OnNetworkInitialize()
        {
            MapManager.Instance.AddObject(this);
        }

        public override void OnDestroyed()
        {
            MapManager.Instance.AddObject(this);
        }
    }
}

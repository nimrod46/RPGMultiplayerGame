using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.MapObjects
{
    public class Chest : CloseOpenBlock
    {
        public Chest() : base(GraphicManager.Instance.AnimationsByDoors[GraphicManager.DoorType.Chest])
        {
        }

        public override bool Isblocking()
        {
            return true;
        }
    }
}

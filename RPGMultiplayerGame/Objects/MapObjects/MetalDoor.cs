using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.MapObjects
{
    public class MetalDoor : Door
    {
        public MetalDoor() : base(GraphicManager.Instance.AnimationsByDoors[GraphicManager.DoorType.MetalDoor])
        {
        }    
    }
}

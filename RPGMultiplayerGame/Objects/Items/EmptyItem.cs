using RPGMultiplayerGame.Objects.InventoryObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items
{
    public class EmptyItem : GameItem
    {
        public EmptyItem() : base(ItemType.None, "")
        {
        }
    }
}

using RPGMultiplayerGame.Objects.InventoryObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items
{
    public class EmptyItem : Item
    {
        public EmptyItem() : base(Inventory.ItemType.None)
        {
        }
    }
}

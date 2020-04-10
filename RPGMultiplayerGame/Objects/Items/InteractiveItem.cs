using RPGMultiplayerGame.Objects.InventoryObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items
{
    public abstract class InteractiveItem : Item
    {
        public InteractiveItem(Inventory.ItemType itemType) : base(itemType)
        {
        }
    }
}

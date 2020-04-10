using RPGMultiplayerGame.Objects.InventoryObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Potions
{
    public class CommonHealthPotion : HealthPotion
    {
        public CommonHealthPotion(int count) : base(Inventory.ItemType.CommonHealthPotion, count, 20)
        {
        }
    }
}

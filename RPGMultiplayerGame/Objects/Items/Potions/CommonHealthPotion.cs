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
        public CommonHealthPotion() : base(ItemType.CommonHealthPotion, "Common health potion", 1, 20)
        {
        }
    }
}

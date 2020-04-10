using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class CommonSword : MeleeWeapon
    {
        public CommonSword() : base(ItemType.CommonSword, new Point(7, 6), 50, "Common sword", 0)
        {

        }
    }
}

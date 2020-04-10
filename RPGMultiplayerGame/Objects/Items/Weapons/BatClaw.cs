using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    class BatClaw : MeleeWeapon
    {
        public BatClaw() : base(ItemType.BatClaw, new Point(5, 5), 5, "Bat claw", 2.5)
        {
        }
    }
}

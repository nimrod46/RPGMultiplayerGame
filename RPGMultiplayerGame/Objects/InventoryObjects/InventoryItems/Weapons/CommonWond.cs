using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.InventoryObjects.InventoryItems.Weapons
{
    public class CommonWond : RangedWeapon
    {
        public CommonWond() : base(InventoryItemType.CommonWond ,new Point(0,0), 25, "Common Wand", new FireBall())
        {
        }
    }
}

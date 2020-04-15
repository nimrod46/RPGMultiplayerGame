using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class CommonWond : RangedWeapon
    {
        public CommonWond() : base(ItemType.CommonWond, new Point(0, 0), 50, "Common Wand", new FireBall(), 2)
        {
        }
    }
}

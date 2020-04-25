using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class CommonSword : MeleeWeapon
    {
        public CommonSword() : base(ItemType.CommonSword, "Common sword", new Point(7, 6), 5, 0)
        {

        }
    }
}

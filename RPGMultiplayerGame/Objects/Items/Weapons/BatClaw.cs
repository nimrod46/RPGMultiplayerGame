using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    class BatClaw : MeleeWeapon
    {
        public BatClaw() : base(ItemType.BatClaw, "Bat claw", new Point(5, 5), 5, 2.5)
        {
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Weapons
{
    public class Wand : RangedWeapon
    {
        public Wand() : base(new Point(0,0), 25, "Wand", new FireBall())
        {
        }
    }
}

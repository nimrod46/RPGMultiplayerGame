using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponEffects
{
    public class FireBall : WeaponEffect
    {
        public FireBall() : base(GameManager.EffectId.FireBall)
        {
        }
    }
}

﻿using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions
{
    public class CommonArrow : Arrow
    {
        public CommonArrow() : base(GraphicManager.WeaponAmmunitionId.CommonArrow, int.MaxValue)
        {
            Scale *= 0.5f;
        }
    }
}

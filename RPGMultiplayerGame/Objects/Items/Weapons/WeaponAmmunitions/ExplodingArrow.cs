﻿using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions
{
    public class ExplodingArrow : Arrow
    {
        public ExplodingArrow() : base(GraphicManager.WeaponAmmunitionId.ExplodingArrow, 1)
        {
            scale *= 0.5f;
        }
    }
}
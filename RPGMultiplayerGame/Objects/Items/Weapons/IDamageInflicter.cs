﻿using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public interface IDamageInflicter
    {
        float Damage { get; set; }
        Direction Direction { get; set; }

        void Attack(Entity attacker);

        void Hit(Entity attacker, Entity victim);

    }
}

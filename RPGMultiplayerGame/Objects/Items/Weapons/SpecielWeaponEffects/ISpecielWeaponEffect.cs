using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects
{
    public interface ISpecielWeaponEffect : IGameUpdateable
    {
        bool AllowMultiple { get; set; }

        void Activated();

        void Update();

        void End();

    }
}

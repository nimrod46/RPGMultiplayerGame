using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.Weapons
{
    class WeaponEffect : AnimatedObject
    {
        protected EffectId effectId;

        public WeaponEffect(EffectId effectId) : base(new Dictionary<int, List<GameTexture>>(GameManager.Instance.animationsByEffects[effectId]))
        {
            this.effectId = effectId;
        }

        protected override void InitAnimationsList()
        {

        }
    }
}

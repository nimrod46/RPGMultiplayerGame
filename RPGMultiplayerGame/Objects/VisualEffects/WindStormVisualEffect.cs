using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.VisualEffects
{
    public class WindStormVisualEffect : VisualEffect
    {
        public WindStormVisualEffect() : base(GraphicManager.Instance.AnimationsByVisualEffect[VisualEffectId.WindStorm])
        {
        }
    }
}

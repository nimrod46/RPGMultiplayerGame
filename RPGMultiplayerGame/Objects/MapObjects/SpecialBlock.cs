using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.MapObjects
{
    public abstract class SpecialBlock : Block
    {
        public SpecialBlock(Dictionary<int, List<GameTexture>> animationsByType) : base(animationsByType)
        {
        }

        public abstract void CmdEngage(Entity initiator);

        public abstract bool Isblocking();
    }
}

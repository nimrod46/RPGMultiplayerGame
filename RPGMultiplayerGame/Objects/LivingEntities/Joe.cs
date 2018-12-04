using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    class Joe : Npc
    {
        public Joe() : base(GameManager.EntityID.Player, 0, 0, 100, GameManager.Instance.PlayerName)
        {
            syncName = "Joe";
        }
    }
}

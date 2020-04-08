using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class Bat : Monster
    {
        public Bat() : base(GameManager.EntityId.Bat, 0, 0, 100, GameManager.Instance.PlayerName)
        {
            SyncName = "Bat";
            animationTimeDelay *= 0.7;
        }      
    }
}

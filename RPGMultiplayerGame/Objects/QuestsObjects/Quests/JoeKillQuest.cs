using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.QuestsObjects.Quests
{
    public class JoeKillQuest : KillQuest
    {
        public JoeKillQuest() : base(nameof(Joe), "Kill 5 bats", EntityId.Bat, 5)
        {
        }
    }
}

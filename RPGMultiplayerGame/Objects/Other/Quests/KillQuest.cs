using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.Other.Quests
{
    public class KillQuest : Quest
    {
        private readonly EntityId entityId;
        private readonly int numberOfKills;
        private int currentKills;
        public KillQuest(string text, EntityId entityId, int numberOfKills) : base(text)
        {
            this.entityId = entityId;
            this.numberOfKills = numberOfKills;
            currentKills = 0;
        }

        public override void AssignTo(Player player)
        {
            base.AssignTo(player);
            player.OnEnitytKillEvent += OnEnitytKill;
        }

        private void OnEnitytKill(Entity killedEntity)
        {
            Console.WriteLine(currentKills);
            if(killedEntity.entityId == entityId)
            {
                if(currentKills != numberOfKills - 1)
                {
                    currentKills++;
                }
                else
                {
                    player.OnEnitytKillEvent -= OnEnitytKill;
                    IsFinished = true;
                }
            }
        }
    }
}

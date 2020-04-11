using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.QuestsObjects
{
    public class KillQuest : Quest
    {
        protected int CurrentKills
        {
            get => currentKills; set
            {
                currentKills = value;
                InvokeSyncVarNetworkly(nameof(currentKills), currentKills);
            }
        }

        private readonly EntityId entityId;
        private readonly int numberOfKills;
        private int currentKills;
        public KillQuest(string npcName, string text, EntityId entityId, int numberOfKills) : base(npcName, text)
        {
            this.entityId = entityId;
            this.numberOfKills = numberOfKills;
            CurrentKills = 0;
        }


        public override void AssignTo(Player player)
        {
            base.AssignTo(player);
            player.OnEnitytKillEvent += OnEnitytKill;
        }

        private void OnEnitytKill(Entity killedEntity)
        {
            Console.WriteLine(CurrentKills);
            if (killedEntity.entityId == entityId)
            {
                if (CurrentKills != numberOfKills - 1)
                {
                    CurrentKills++;
                }
                else
                {
                    player.OnEnitytKillEvent -= OnEnitytKill;
                    SyncIsFinished = true;
                }
            }
        }
    }
}

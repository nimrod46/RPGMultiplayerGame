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
        protected int SyncCurrentKills
        {
            get => currentKills; set
            {
                currentKills = value;
                InvokeSyncVarNetworkly(nameof(SyncCurrentKills), currentKills);
            }
        }

        private readonly EntityId entityId;
        private readonly int numberOfKills;
        private int currentKills;
        public KillQuest(string npcName, string text, EntityId entityId, int numberOfKills) : base(npcName, text)
        {
            this.entityId = entityId;
            this.numberOfKills = numberOfKills;
            SyncCurrentKills = 0;
        }


        public override void AssignTo(Player player)
        {
            base.AssignTo(player);
            player.OnEnitytKillEvent += OnEnitytKill;
        }

        private void OnEnitytKill(Entity killedEntity)
        {
            Console.WriteLine(SyncCurrentKills);
            if (killedEntity.EntityId == entityId)
            {
                if (SyncCurrentKills != numberOfKills - 1)
                {
                    SyncCurrentKills++;
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

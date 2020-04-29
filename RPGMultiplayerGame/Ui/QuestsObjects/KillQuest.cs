using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using static RPGMultiplayerGame.Managers.GraphicManager;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Objects.QuestsObjects
{
    public class KillQuest : Quest
    {
        public int SyncCurrentKills
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
        public KillQuest(string npcName, string text, Action<Player> reward, EntityId entityId, int numberOfKills) : base(npcName, text, reward)
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

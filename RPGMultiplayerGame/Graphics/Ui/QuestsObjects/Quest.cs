using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.Graphics.Ui.QuestsObjects;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects.Quests;
using RPGMultiplayerGame.Ui;
using System;
using System.Xml.Serialization;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Objects.QuestsObjects
{
    [XmlInclude(typeof(JoeKillQuest))]
    public class Quest : NetworkIdentity
    {
        public bool SyncIsFinished
        {
            get => isFinished;
            set
            {
                isFinished = value;
                InvokeSyncVarNetworkly(nameof(SyncIsFinished), isFinished);
                if (isFinished)
                {
                    QuestUi?.MarkFinished();
                }
            }
        }

        [XmlIgnore]
        public QuestUi QuestUi { get; set; }

        public string NpcName { get; }

        private readonly PositionType positionType;
        private readonly Func<Point, Vector2> origin;
        protected Player player;
        private readonly string text;
        private readonly Action<QuestReward, Player> reward;
        private Color textColor;
        private bool isFinished;
        private readonly QuestReward questReward;

        private Quest()
        {

        }

        public Quest(string npcName, string text, Action<QuestReward, Player> reward)
        {
            this.origin = (windowSize) => Vector2.Zero;
            this.positionType = PositionType.TopLeft;
            this.NpcName = npcName;
            this.text = text;
            this.reward = reward;
            textColor = Color.Blue;
            SyncIsFinished = false;
            questReward = new QuestReward();
            OnNetworkInitializeEvent += OnNetworkInitialize;
            OnDestroyEvent += OnDestroy;
        }

        private void OnNetworkInitialize()
        {
            QuestUi = new QuestUi(origin, positionType, NpcName, text, textColor);
            if (SyncIsFinished)
            {
                QuestUi.MarkFinished();
            }
        }

        public virtual void AssignTo(Player player)
        {
            this.player = player;
            player.InvokeCommandMethodNetworkly(nameof(player.AddQuest), this);
        }

        public virtual void Unassign()
        {
            player.InvokeCommandMethodNetworkly(nameof(player.RemoveQuest), this);
            InvokeBroadcastMethodNetworkly(nameof(Destroy));
        }

        public virtual void RewardPlayer()
        {
            reward.Invoke(questReward,player);
        }

        public void MakeVisible()
        {
            QuestUi.IsVisible = true;
        }

        private void OnDestroy(NetworkIdentity identity)
        {
            QuestUi.IsVisible = false;
        }
    }
}

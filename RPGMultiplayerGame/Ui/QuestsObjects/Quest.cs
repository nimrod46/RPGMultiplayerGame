using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Objects.QuestsObjects
{
    public class Quest : NetworkIdentity
    {
        public bool SyncIsFinished
        {
            get => isFinished;
            protected set
            {
                isFinished = value;
                InvokeSyncVarNetworkly(nameof(SyncIsFinished), isFinished);
                if (isFinished)
                {
                    QuestUi?.MarkFinished();
                }
            }
        }

        private readonly PositionType positionType;
        private readonly Func<Point, Vector2> origin;
        protected Player player;
        public QuestUi QuestUi { get; set; }
        private readonly string npcName;
        private readonly string text;
        private readonly Action<Player> reward;
        private Color textColor;
        private bool isFinished;

        public Quest(string npcName, string text, Action<Player> reward)
        {
            this.origin = (windowSize) => Vector2.Zero;
            this.positionType = PositionType.TopLeft;
            this.npcName = npcName;
            this.text = text;
            this.reward = reward;
            textColor = Color.Blue;
            SyncIsFinished = false;
            OnNetworkInitializeEvent += OnNetworkInitialize;
            OnDestroyEvent += OnDestroy;
        }

        private void OnNetworkInitialize()
        {
            QuestUi = new QuestUi(origin, positionType, npcName, text, textColor);
            if(SyncIsFinished)
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
            ServerManager.Instance.RemoveQuest(player, this);
            InvokeBroadcastMethodNetworkly(nameof(Destroy));
        }

        public virtual void RewardPlayer()
        {
            reward.Invoke(player);
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

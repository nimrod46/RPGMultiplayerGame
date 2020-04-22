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
                    questUi.MarkFinished();
                }
            }
        }

        public Vector2 Position
        {
            get { return questUi.Position; }
            set
            {
                questUi.Position = value;
            }
        }

        public Vector2 Origin
        {
            get { return questUi.Origin; }
            set
            {
                questUi.Origin = value;
            }
        }

        public Vector2 Size
        {
            get { return questUi.Size; }
            set
            {
                questUi.Size = value;
            }
        }

        public PositionType PositionType
        {
            get { return questUi.OriginType; }
            set
            {
                questUi.OriginType = value;
            }
        }

        private readonly PositionType positionType;
        private readonly Func<Point, Vector2> origin;
        protected Player player;
        private QuestUi questUi;
        private readonly string npcName;
        private readonly string text;
        private readonly Action<Player> reward;
        private Color textColor;
        private bool isFinished;

        public Quest(Func<Point, Vector2> origin, PositionType positionType, string npcName, string text, Action<Player> reward)
        {
            this.origin = origin;
            this.positionType = positionType;
            this.npcName = npcName;
            this.text = text;
            this.reward = reward;
            textColor = Color.Blue;
            SyncIsFinished = false;
            OnNetworkInitializeEvent += OnNetworkInitialize;
        }

        private void OnNetworkInitialize()
        {
            questUi = new QuestUi(origin, positionType, npcName, text, textColor);
        }


        public virtual void AssignTo(Player player)
        {
            this.player = player;
            player.AddQuest(this);
        }

        public virtual void Unassign(Player player)
        {
            player.RemoveQuest(this);
        }

        public virtual void RewardPlayer(Player player)
        {
            reward.Invoke(player);
        }

        public void Draw(SpriteBatch sprite)
        {
            questUi.Draw(sprite);
        }
    }
}

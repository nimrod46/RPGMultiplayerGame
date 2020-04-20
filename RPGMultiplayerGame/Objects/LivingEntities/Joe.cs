using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Dialogs;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Objects.QuestsObjects;
using RPGMultiplayerGame.Objects.QuestsObjects.Quests;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    class Joe : MultipleInteractionNpc
    {
        public Joe() : base(GameManager.EntityId.Player, 0, 0, 100, GameManager.Instance.PlayerNameFont)
        {
            SyncName = "Joe";
            minDistanceForObjectInteraction = 40;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            dialog = new ComplexDialog(SyncName, "Hi! can you help me?", false);
            dialog
                .AddAnswerOption("Yes", "Thank You!", false)
                .AddAnswerOption("....", new QuestDialog("Lets get started", new JoeKillQuest(), "So you need to kill for me some bats", "Tell me when you are done", "Ammmm.... seams like you did not finished", "Good job!! here is your reward"))
                .AddAnswerOption("Thank you", "Be well", true);
            dialog.AddAnswerOption("No", "Ok", false);
        }
    }
}

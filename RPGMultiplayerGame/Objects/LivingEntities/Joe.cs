using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Objects.Other.Quests;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    class Joe : Npc
    {
        private bool isInDialog;

        public Joe() : base(GameManager.EntityId.Player, 0, 0, 100, GameManager.Instance.PlayerNameFont)
        {
            SyncName = "Joe";
            isInDialog = false;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            dialog = new ComplexDialog(SyncName, "Hi! can you help me?");
            dialog.AddAnswerOption("Yes", "Thank You!").AddAnswerOption("....", "Lets get started").AddAnswerOption<QuestDialog>("Got it","So you need to kill for me some bats",new KillQuest("Kill 5 bats", EntityId.Bat, 5));
            dialog.AddAnswerOption("No", "Ok");
        }
        public override void InteractWithPlayer(Player player)
        {
            if(!isInDialog)
            {
                isInDialog = true;
                BroadCastInteractWithPlayer(player);
            }
        }

        private void BroadCastInteractWithPlayer(Player player)
        {
            InvokeBroadcastMethodNetworkly(nameof(BroadCastInteractWithPlayer), player);
            isInDialog = true;
            if (player.hasAuthority)
            {
                player.InteractWithNpc(this);
                currentDialog = dialog;
                currentInteractingPlayer = player;
            }
            else
            {
                currentSimpleDialog = new SimpleDialog(dialog.Text);
            }
        }

        internal override void ChooseDialogOption(int index)
        {
            currentDialog = currentDialog.GetNextDialogByAnswer(currentInteractingPlayer, index);
            if (currentDialog == null)
            {
                StopInteractWithPlayer(currentInteractingPlayer);
            }
            else
            {
                ShowSimpleDialog(currentDialog.Text);
            }
        }

        private void ShowSimpleDialog(string text)
        {
            InvokeBroadcastMethodNetworkly(nameof(ShowSimpleDialog), text);
            if (currentInteractingPlayer == null || !currentInteractingPlayer.hasAuthority)
            {
                currentSimpleDialog = new SimpleDialog(text);
            }
        }
        
        public override void StopInteractWithPlayer(Player player)
        {
            base.StopInteractWithPlayer(player);
            isInDialog = false;
        }
    }
}

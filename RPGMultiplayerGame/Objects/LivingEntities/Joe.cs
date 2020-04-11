using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Objects.QuestsObjects;
using RPGMultiplayerGame.Objects.QuestsObjects.Quests;
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
            dialog.AddAnswerOption("Yes", "Thank You!").AddAnswerOption("....", "Lets get started").AddAnswerOption<QuestDialog>("Got it","So you need to kill for me some bats", new JoeKillQuest());
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
            if (isInServer || player.hasAuthority)
            {
                currentInteractingPlayer = player;
                currentDialog = dialog;
                if (player.hasAuthority)
                {
                    currentInteractingPlayer.InteractWithNpc(this);
                }
            }
            else
            {
                currentSimpleDialog = new SimpleDialog(dialog.Text);
            }
        }

        internal override void ChooseDialogOption(int index)
        {
            InvokeCommandMethodNetworkly(nameof(ChooseDialogOption), index);
            if (!isInServer)
            {
                return;
            }
            currentDialog = currentDialog.GetNextDialogByAnswer(currentInteractingPlayer, index);
            if (currentDialog == null)
            {
                StopInteractWithPlayer(currentInteractingPlayer);
            }
            else
            {
                ShowNextDialog(currentDialog.Text, index);
            }
        }

        private void ShowNextDialog(string text, int index)
        {
            InvokeBroadcastMethodNetworkly(nameof(ShowNextDialog), text, index);
            if (currentInteractingPlayer == null || !currentInteractingPlayer.hasAuthority)
            {
                currentSimpleDialog = new SimpleDialog(text);
            }
            else if(!isInServer)
            {
                currentDialog = currentDialog.GetNextDialogByAnswer(currentInteractingPlayer, index);
            }
        }

        public override void StopInteractWithPlayer(Player player)
        {
            InvokeBroadcastMethodNetworkly(nameof(StopInteractWithPlayer), player);
            if (isInServer || player.hasAuthority)
            {
                if (player.hasAuthority)
                {
                    currentInteractingPlayer.StopInteractingWithNpc();
                }
                currentInteractingPlayer = null;
                currentDialog = null;
            }
            currentSimpleDialog = null;
            isInDialog = false;
        }
    }
}

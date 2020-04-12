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
        private readonly Dictionary<string, ComplexDialog> curentInteractingPlayers = new Dictionary<string, ComplexDialog>();
        private readonly Dictionary<string, int> playersProgres = new Dictionary<string, int>();
        public Joe() : base(GameManager.EntityId.Player, 0, 0, 100, GameManager.Instance.PlayerNameFont)
        {
            SyncName = "Joe";
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            dialog = new ComplexDialog(SyncName, "Hi! can you help me?", false);
            dialog
                .AddAnswerOption("Yes", "Thank You!", false)
                    .AddAnswerOption("....", "Lets get started", false)
                        .AddAnswerOption<QuestDialog>("Got it", "So you need to kill for me some bats", new JoeKillQuest(), "Tell me when you are done");
            dialog.AddAnswerOption("No", "Ok", false);
        }

        public override void InteractWithPlayer(Player player)
        {
            if (curentInteractingPlayers.ContainsKey(player.GetName()))
            {
                return;
            }

            ComplexDialog progresDialog;
            if (!playersProgres.ContainsKey(player.GetName()))
            {
                playersProgres.Add(player.GetName(), dialog.Index);
                progresDialog = dialog;
            }
            else
            {
                progresDialog = dialog.GetDialogByIndex(playersProgres[player.GetName()]);
            }
            curentInteractingPlayers.Add(player.GetName(), progresDialog);
            CmdInteractWithPlayer(player, progresDialog.Index);
        }

        private void CmdInteractWithPlayer(Player player, int dialogIndex)
        {
            InvokeCommandMethodNetworkly(nameof(CmdInteractWithPlayer), player.OwnerId, player, dialogIndex);
            if (isInServer)
            {
                return;
            }
            Console.WriteLine(dialog.GetDialogByIndex(dialogIndex).Text);
            currentDialog = dialog.GetDialogByIndex(dialogIndex);
            player.InteractWithNpc(this);
        }

        internal override void CmdChooseDialogOption(Player player, int answerIndex)
        {
            InvokeCommandMethodNetworkly(nameof(CmdChooseDialogOption), player, answerIndex);
            if (!isInServer)
            {
                return;
            }
            currentDialog = curentInteractingPlayers[player.GetName()];
            currentDialog = currentDialog.GetNextDialogByAnswer(player, answerIndex);
            if (currentDialog == null)
            {
                CmdStopInteractWithPlayer(player);
            }
            else
            {
                if(currentDialog.IsProgressing)
                {
                    playersProgres[player.GetName()] = currentDialog.Index;
                }
                curentInteractingPlayers[player.GetName()] = currentDialog;
                CmdShowNextDialogForPlayer(player, answerIndex);
            }
        }

        private void CmdShowNextDialogForPlayer(Player player, int answerIndex)
        {
            InvokeCommandMethodNetworkly(nameof(CmdShowNextDialogForPlayer), player.OwnerId, player, answerIndex);
            if (isInServer)
            {
                return;
            }

            currentDialog = currentDialog.GetNextDialogByAnswer(player, answerIndex);
        }

        public override void CmdStopInteractWithPlayer(Player player)
        {
            InvokeCommandMethodNetworkly(nameof(CmdStopInteractWithPlayer), player.OwnerId, player);
            if (isInServer)
            {
                curentInteractingPlayers.Remove(player.GetName());
                return;
            }
            if(currentDialog == null)
            {
                return;
            }

            player.StopInteractingWithNpc();
            currentDialog = null;
        }
    }
}

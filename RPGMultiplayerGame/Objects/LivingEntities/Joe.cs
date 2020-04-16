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
    class Joe : Npc
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
                .AddAnswerOption("....", "Lets get started", false)
                .AddAnswerOption<DialogQuestAssign>("Got it", "So you need to kill for me some bats", new JoeKillQuest())
                .AddAnswerOption<DialogQuestInProgress>("Okay", "Tell me when you are done", "Ammm seams like you didn't finish, do you need anything?")
                .AddAnswerOption<DialogQuestCompleted>("Ok finished", "Good job!!! Here is your reward")
                .AddAnswerOption<ComplexDialog>("Thank you", "Be well", true);
            dialog.AddAnswerOption("No", "Ok", false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!hasAuthority)
            {
                return;
            }

            List<Player> currentInteractingPlayers = GetCurrentPlayersInRadius();
            
            foreach (var player in curentInteractingPlayersDialogs.Keys.ToList().Where(pl => !currentInteractingPlayers.Contains(pl)))
            {
               InvokeBroadcastMethodNetworkly(nameof(StopLookingAtGameObject), player);
            }

            foreach (var player in currentInteractingPlayers)
            {
                InvokeBroadcastMethodNetworkly(nameof(LookAtGameObject), player);
            }
        }

        protected override void LookAtGameObject(GameObject gameObject)
        {
            if (gameObject is Player player)
            {
                if (isInServer)
                {
                    InteractWithPlayer(player);
                }
                else if(ClientManager.Instance.Player.IsInteractingWith(this) && !player.hasAuthority)
                {
                    return;
                }
            }
            base.LookAtGameObject(gameObject);
        }

        protected override void StopLookingAtGameObject(GameObject gameObject)
        {
            if (gameObject is Player player)
            {
                if (isInServer)
                {
                    CmdStopInteractWithPlayer(player);
                }
                else if (ClientManager.Instance.Player.IsInteractingWith(this) && !player.hasAuthority)
                {
                    return;
                }
            }
            base.StopLookingAtGameObject(gameObject);
        }

        public override void InteractWithPlayer(Player player)
        {
            if (curentInteractingPlayersDialogs.ContainsKey(player))
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
            curentInteractingPlayersDialogs.Add(player, progresDialog);
            CmdRequestingInteractWithPlayer(player, progresDialog.Index);
        }

        public override void CmdRequestingInteractWithPlayer(Player player, int dialogIndex)
        {
            InvokeCommandMethodNetworkly(nameof(CmdRequestingInteractWithPlayer), player.OwnerId, player, dialogIndex);
            if (isInServer)
            {
                return;
            }
            currentSimpleDialog = dialog.GetDialogByIndex(dialogIndex);
            player.InteractRequestWithNpc(this);
        }

        public override void CmdAcceptInteractWithPlayer(Player player)
        {
            InvokeCommandMethodNetworkly(nameof(CmdAcceptInteractWithPlayer), player);
            if (!isInServer)
            {
                return;
            }
            CmdInteractWithPlayer(player, curentInteractingPlayersDialogs[player].Index);
        }
       

        private void CmdInteractWithPlayer(Player player, int dialogIndex)
        {
            InvokeCommandMethodNetworkly(nameof(CmdInteractWithPlayer), player.OwnerId, player, dialogIndex);
            if (isInServer)
            {
                return;
            }
            currentComplexDialog = dialog.GetDialogByIndex(dialogIndex);
            player.InteractWithNpc(this);
        }

        public override void CmdChooseDialogOption(Player player, int answerIndex)
        {
            InvokeCommandMethodNetworkly(nameof(CmdChooseDialogOption), player, answerIndex);
            if (!isInServer)
            {
                return;
            }
            currentComplexDialog = curentInteractingPlayersDialogs[player];
            currentComplexDialog = currentComplexDialog.GetNextDialogByAnswer(player, answerIndex);
            if (currentComplexDialog == null)
            {
                CmdStopInteractWithPlayer(player);
            }
            else
            {
                if(currentComplexDialog.IsProgressing)
                {
                    playersProgres[player.GetName()] = currentComplexDialog.Index;
                }
                curentInteractingPlayersDialogs[player] = currentComplexDialog;
                CmdShowNextDialogForPlayer(player, currentComplexDialog.Index);
            }
        }

        private void CmdShowNextDialogForPlayer(Player player, int dialogIndex)
        {
            InvokeCommandMethodNetworkly(nameof(CmdShowNextDialogForPlayer), player.OwnerId, player, dialogIndex);
            if (isInServer)
            {
                return;
            }

            currentComplexDialog = dialog.GetDialogByIndex(dialogIndex);
        }

        public override void CmdStopInteractWithPlayer(Player player)
        {
            InvokeCommandMethodNetworkly(nameof(CmdStopInteractWithPlayer), player.OwnerId, player);
            if (isInServer)
            {
                curentInteractingPlayersDialogs.Remove(player);
                return;
            }
            if (currentComplexDialog == null && currentSimpleDialog == null)
            {
                return;
            }

            player.StopInteractingWithNpc();
            currentComplexDialog = null;
            currentSimpleDialog = null;
        }
    }
}

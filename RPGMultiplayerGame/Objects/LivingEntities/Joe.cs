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
        private bool isCurrentAthorityPlayerInteracting;

        public Joe() : base(GameManager.EntityId.Player, 0, 0, 100, GameManager.Instance.PlayerNameFont)
        {
            SyncName = "Joe";
            isCurrentAthorityPlayerInteracting = false;
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

                Player player = ClientManager.Instance.Player;
                if(player == null)
                {
                    return;
                }
                if (IsObjectInInteractingRadius(player))
                {
                    isCurrentAthorityPlayerInteracting = true;
                    LookAtGameObject(player);
                }
                else if(isCurrentAthorityPlayerInteracting)
                {
                    isCurrentAthorityPlayerInteracting = false;
                }
                return;
            }

            Player lastInteractingPlayer = null;
            List<Player> currentInteractingPlayers = GetCurrentPlayersInRadius();
            if (currentInteractingPlayers.Count != 0)
            {
                lastInteractingPlayer = currentInteractingPlayers[0];
            }

            foreach (var player in curentInteractingPlayersDialogs.Keys.ToList().Where(pl => !currentInteractingPlayers.Contains(pl)))
            {
               InvokeBroadcastMethodNetworkly(nameof(StopLookingAtGameObject), player);
            }
            

            if (lastInteractingPlayer != null)
            {
                InvokeBroadcastMethodNetworkly(nameof(LookAtGameObject), lastInteractingPlayer);
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
                else if(isCurrentAthorityPlayerInteracting && !player.hasAuthority)
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
                else if (isCurrentAthorityPlayerInteracting && !player.hasAuthority)
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
            CmdInteractWithPlayer(player, progresDialog.Index);
        }

        private void CmdInteractWithPlayer(Player player, int dialogIndex)
        {
            InvokeCommandMethodNetworkly(nameof(CmdInteractWithPlayer), player.OwnerId, player, dialogIndex);
            if (isInServer)
            {
                return;
            }
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
            currentDialog = curentInteractingPlayersDialogs[player];
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
                curentInteractingPlayersDialogs[player] = currentDialog;
                CmdShowNextDialogForPlayer(player, currentDialog.Index);
            }
        }

        private void CmdShowNextDialogForPlayer(Player player, int dialogIndex)
        {
            InvokeCommandMethodNetworkly(nameof(CmdShowNextDialogForPlayer), player.OwnerId, player, dialogIndex);
            if (isInServer)
            {
                return;
            }

            currentDialog = dialog.GetDialogByIndex(dialogIndex);
        }

        public override void CmdStopInteractWithPlayer(Player player)
        {
            InvokeCommandMethodNetworkly(nameof(CmdStopInteractWithPlayer), player.OwnerId, player);
            if (isInServer)
            {
                curentInteractingPlayersDialogs.Remove(player);
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

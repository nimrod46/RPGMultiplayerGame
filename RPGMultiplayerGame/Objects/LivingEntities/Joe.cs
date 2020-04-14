﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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
        private bool isCurrentAthorityPlayerInteracting;
        public Joe() : base(GameManager.EntityId.Player, 0, 0, 100, GameManager.Instance.PlayerNameFont)
        {
            SyncName = "Joe";
            isCurrentAthorityPlayerInteracting = false;
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!hasAuthority)
            {
                return;
            }

            List<Player> newInteractingPlayers = new List<Player>();
            List<Player> stoppedInteractingPlayers = new List<Player>();
            for (int i = 0; i < ServerManager.Instance.players.Count; i++)
            {
                Player player = ServerManager.Instance.players[i];
                float distance = Vector2.Distance(player.GetBaseCenter(), GetBaseCenter());

                if (distance < minDistanceForPlayerInteraction)
                {
                    if (!interactingPlayers.Contains(player))
                    {
                        interactingPlayers.Add(player);
                        newInteractingPlayers.Add(player);
                    }
                }
                else if (interactingPlayers.Contains(player))
                {
                    interactingPlayers.Remove(player);
                    stoppedInteractingPlayers.Add(player);
                }
            }
            foreach (var player in stoppedInteractingPlayers)
            {
                InvokeBroadcastMethodNetworkly(nameof(StopLookingAtGameObject), player);
            }

            foreach (var player in interactingPlayers)
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
                else if(player.hasAuthority)
                {
                    isCurrentAthorityPlayerInteracting = true;
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
                else if (player.hasAuthority)
                {
                    isCurrentAthorityPlayerInteracting = false;
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

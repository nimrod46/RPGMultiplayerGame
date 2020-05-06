using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Dialogs;
using RPGMultiplayerGame.Objects.Other;
using System.Collections.Generic;
using System.Linq;
using System;
using RPGMultiplayerGame.Objects.QuestsObjects;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public abstract class MultipleInteractionNpc : Npc
    {
        public MultipleInteractionNpc(GraphicManager.EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, nameFont)
        {
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
                StopLookingAtGameObject(player);
            }

            foreach (var player in currentInteractingPlayers)
            {
                LookAtGameObject(player, (int)State.Idle);
            }
        }

        protected override void LookAtGameObject(GameObject gameObject, int entityState)
        {
            if (gameObject is Player player)
            {
                InteractWithPlayer(player);
            }
            base.LookAtGameObject(gameObject, entityState);
        }

        protected override void StopLookingAtGameObject(GameObject gameObject)
        {
            if (gameObject is Player player)
            {
                CmdStopInteractWithPlayer(player);
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
            if (!playersProgress.ContainsKey(player.GetName()))
            {
                playersProgress.Add(player.GetName(), dialog.Index);
                progresDialog = dialog;
            }
            else
            {
                progresDialog = dialog.GetDialogByIndex(playersProgress[player.GetName()]);
            }
            curentInteractingPlayersDialogs.Add(player, progresDialog);
            InvokeCommandMethodNetworkly(nameof(CmdRequestingInteractWithPlayer), player.OwnerId, player, progresDialog.Index);
        }

        public override void CmdRequestingInteractWithPlayer(Player player, int dialogIndex)
        {
            currentSimpleDialog = new InteractionText(dialog.GetDialogByIndex(dialogIndex).Text)
            {
                Parent = this,
                DrawOffset = dialogOffset
            };
            player.InteractRequestWithNpc(this);
        }

        public override void InteractionAcceptedByPlayer(Player player)
        {
            InvokeCommandMethodNetworkly(nameof(CmdInteractWithPlayer), player.OwnerId, player, curentInteractingPlayersDialogs[player].Index);
        }

        protected virtual void CmdInteractWithPlayer(Player player, int dialogIndex)
        {

            currentComplexDialog = dialog.GetDialogByIndex(dialogIndex);
            currentComplexDialog.IsVisible = true;
            player.InteractWithNpc(this);
        }

        public override void CmdChooseDialogOption(Player player, int answerIndex)
        {
            currentComplexDialog = curentInteractingPlayersDialogs[player];
            currentComplexDialog = currentComplexDialog.GetNextDialogByAnswer(player, answerIndex);
            if (currentComplexDialog == null)
            {
                CmdStopInteractWithPlayer(player);
            }
            else
            {
                if (currentComplexDialog.IsProgressing)
                {
                    playersProgress[player.GetName()] = currentComplexDialog.Index;
                }
                curentInteractingPlayersDialogs[player] = currentComplexDialog;
                InvokeCommandMethodNetworkly(nameof(CmdShowNextDialogForPlayer), player.OwnerId, currentComplexDialog.Index);
            }
        }

        protected void CmdShowNextDialogForPlayer(int dialogIndex)
        {
            currentSimpleDialog = new InteractionText(dialog.GetDialogByIndex(dialogIndex).Text)
            {
                Parent = this,
                DrawOffset = dialogOffset
            };
            currentComplexDialog.IsVisible = false;
            currentComplexDialog = dialog.GetDialogByIndex(dialogIndex);
            currentComplexDialog.IsVisible = true;
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
            if (currentComplexDialog != null)
            {
                currentComplexDialog.IsVisible = false;
            }

            player.StopInteractingWithNpc();
            currentComplexDialog = null;
            currentSimpleDialog = null;
        }

        public override void AssignQuestTo(Player player, Quest quest)
        {
            Console.WriteLine(dialog.GetDialogByIndex(playersProgress[player.GetName()] - 2));
           (dialog.GetDialogByIndex(playersProgress[player.GetName()] - 2) as QuestDialog).AssignPlayer(player, quest);
        }

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Dialogs;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class MultipleInteractionNpc : Npc
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
                InvokeBroadcastMethodNetworkly(nameof(StopLookingAtGameObject), player);
            }

            foreach (var player in currentInteractingPlayers)
            {
                InvokeBroadcastMethodNetworkly(nameof(LookAtGameObject), player, (int) State.Idle);
            }
        }

        protected override void LookAtGameObject(GameObject gameObject, int entityState)
        {
            if (gameObject is Player player)
            {
                if (isInServer)
                {
                    InteractWithPlayer(player);
                }
                else if (ClientManager.Instance.Player.IsInteractingWith(this) && !player.hasAuthority)
                {
                    return;
                }
            }
            base.LookAtGameObject(gameObject, entityState);
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
                    playersProgres[player.GetName()] = currentComplexDialog.Index;
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
            if(isInServer)
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
    }
}

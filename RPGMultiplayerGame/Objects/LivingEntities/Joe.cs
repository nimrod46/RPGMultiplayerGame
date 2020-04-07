using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    class Joe : Npc
    {
        private bool isInDialog;

        public Joe() : base(GameManager.EntityID.Player, 0, 0, 100, GameManager.Instance.PlayerName)
        {
            syncName = "Joe";
            isInDialog = false;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            dialog = new ComplexDialog(syncName, "Hi! can you help me?")
                .AddAnswerOption("Yes", new ComplexDialog("Thank You!")
                    .AddAnswerOption("-->", new ComplexDialog("Lets get started")))
                .AddAnswerOption("No", new ComplexDialog("Ok"));
        }
        public override void InteractWithPlayer(Player player)
        {
            if(!isInDialog)
            {
                isInDialog = true;
                BroadCastInteractWithPlayer(player);
            }
        }

        [BroadcastMethod]
        private void BroadCastInteractWithPlayer(Player player)
        {
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
            if (currentDialog.AnswersCount() >= index + 1)
            {
                currentDialog = currentDialog.GetNextDialogByAnswer(index);
                ShowSimpleDialog(currentDialog.Text);
            }
            else if (currentDialog.AnswersCount() == 0)
            {
                StopInteractWithPlayer(currentInteractingPlayer);
            }
        }

        [BroadcastMethod]
        private void ShowSimpleDialog(string text)
        {
            if (currentInteractingPlayer == null || !currentInteractingPlayer.hasAuthority)
            {
                currentSimpleDialog = new SimpleDialog(text);
            }
        }
        
        [BroadcastMethod]
        public override void StopInteractWithPlayer(Player player)
        {
            base.StopInteractWithPlayer(player);
            isInDialog = false;
        }
    }
}

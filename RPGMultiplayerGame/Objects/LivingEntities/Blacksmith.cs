using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    class Blacksmith : Npc
    {
        public Blacksmith() : base(GameManager.EntityId.Blacksmith, 0, 0, 100, GameManager.Instance.PlayerNameFont)
        {
            SyncName = "Blacksmith";
            scale = 0.3f;
        }

        public override void InteractWithPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public override void CmdChooseDialogOption(Player player, int index)
        {
            throw new NotImplementedException();
        }

        public override void CmdStopInteractWithPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public override void CmdRequestingInteractWithPlayer(Player player, int dialogIndex)
        {
            throw new NotImplementedException();
        }

        public override void CmdAcceptInteractWithPlayer(Player player)
        {
            throw new NotImplementedException();
        }
    }
}

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
        public Blacksmith() : base(GameManager.EntityID.Blacksmith, 0, 0, 100, GameManager.Instance.PlayerName)
        {
            syncName = "Blacksmith";
            scale = 0.3f;
        }

        public override void InteractWithPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public override void StopInteractWithPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        internal override void ChooseDialogOption(int index)
        {
            throw new NotImplementedException();
        }
    }
}

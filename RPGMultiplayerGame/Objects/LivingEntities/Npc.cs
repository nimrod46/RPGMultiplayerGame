﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Objects.Dialogs;
using RPGMultiplayerGame.Objects.Other;
using System.Collections.Generic;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public abstract class Npc : Human
    {
        protected readonly Dictionary<Player, ComplexDialog> curentInteractingPlayersDialogs = new Dictionary<Player, ComplexDialog>();
        protected readonly Dictionary<string, int> playersProgres = new Dictionary<string, int>();
        protected ComplexDialog dialog;
        protected ComplexDialog currentComplexDialog;
        protected InteractionText currentSimpleDialog;
        protected Vector2 dialogOffset;

        public Npc(EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, nameFont, false, Color.BlueViolet)
        {
            speed *= 0.5f;
        }

        public override void OnNameSet()
        {
            base.OnNameSet();
            dialogOffset = nameOffset + new Vector2(0, -nameFontSize.Y);
        }

        public abstract void CmdRequestingInteractWithPlayer(Player player, int dialogIndex);

        public abstract void InteractionAcceptedByPlayer(Player player);

        public abstract void InteractWithPlayer(Player player);

        public abstract void CmdChooseDialogOption(Player player, int index);

        public abstract void CmdStopInteractWithPlayer(Player player);

       

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            currentSimpleDialog?.Draw(sprite);
        }
    }
}

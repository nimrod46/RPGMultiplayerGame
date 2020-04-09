﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public abstract class InventoryStackableItem : InventoryItem
    {
        public int Count { get; set; }
        private readonly SpriteFont spriteFont;
        public InventoryStackableItem(InventoryItemType itemType, int count) : base(itemType)
        {
            Count = count;
            spriteFont = GameManager.Instance.PlayerName;
        }

        public override void Draw(SpriteBatch sprite, Vector2 location, float layer)
        {
            base.Draw(sprite, location, layer);
            sprite.DrawString(spriteFont, Count + "", location, Color.Orange);
        }
    }
}

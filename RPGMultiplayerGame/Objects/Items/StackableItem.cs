﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.Items
{
    public abstract class StackableGameItem : InteractiveItem
    {
        public int SyncCount
        {
            get => syncCount; set
            {
                syncCount = value;
                InvokeSyncVarNetworkly(nameof(SyncCount), syncCount);
            }
        }

        private readonly SpriteFont spriteFont;
        private Vector2 textSize;
        private int syncCount;

        public StackableGameItem(ItemType itemType, string name, int count) : base(itemType, name)
        {
            this.SyncCount = count;
            spriteFont = UiManager.Instance.StackableItemNumberFont;
            textSize = spriteFont.MeasureString(count.ToString());
        }

        public override void Draw(SpriteBatch sprite, Vector2 location, float layer)
        {
            base.Draw(sprite, location, layer);
            sprite.DrawString(spriteFont, SyncCount + "", location + new Vector2(Texture.Width, Texture.Height) + new Vector2(-textSize.X + 10, -textSize.Y / 2), Color.Orange, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer * 0.1f);
        }

        public void Add(StackableGameItem stackableItemToAdd)
        {
            SyncCount += stackableItemToAdd.SyncCount;
            textSize = spriteFont.MeasureString(SyncCount.ToString());
            stackableItemToAdd.InvokeBroadcastMethodNetworkly(nameof(stackableItemToAdd.Destroy));
        }

        public void Use()
        {
            SyncCount--;
            textSize = spriteFont.MeasureString(SyncCount.ToString());
            if (SyncCount == 0)
            {
                SyncItemType = ItemType.None;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.Items
{
    public abstract class StackableItem : InteractiveItem
    {
        private readonly SpriteFont spriteFont;
        protected int count;
        private Vector2 textSize;
        public StackableItem(ItemType itemType, int count) : base(itemType)
        {
            this.count = count;
            spriteFont = GameManager.Instance.PlayerName;
            textSize = spriteFont.MeasureString(count.ToString());
        }

        public override void Draw(SpriteBatch sprite, Vector2 location, float layer)
        {
            base.Draw(sprite, location, layer);
            sprite.DrawString(spriteFont, count + "", location + new Vector2(Texture.Width, Texture.Height) + new Vector2(-textSize.X + 10, - textSize.Y / 2), Color.Orange);
        }

        public void Add(StackableItem stackableItemToAdd)
        {
            count += stackableItemToAdd.count;
            textSize = spriteFont.MeasureString(count.ToString());
        }

        public void Use()
        {
            count--;
            textSize = spriteFont.MeasureString(count.ToString());
        }

        public bool IsDone()
        {
            return count == 0;
        }
    }
}

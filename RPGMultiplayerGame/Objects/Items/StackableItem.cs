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
    public abstract class StackableItem : Item
    {
        public int Count { get; set; }
        private readonly SpriteFont spriteFont;
        public StackableItem(ItemType itemType, int count) : base(itemType)
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

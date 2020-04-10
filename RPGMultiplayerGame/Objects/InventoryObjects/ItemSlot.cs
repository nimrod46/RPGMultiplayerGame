using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public class ItemSlot
    {
        public Item Item { get; set; }
        public Point Location { get; set; }
        public Point Size { get; }

        private readonly Texture2D inventorySlotBackground;

        public ItemSlot()
        {
            inventorySlotBackground = GameManager.Instance.InventorySlotBackground;
            Size = inventorySlotBackground.Bounds.Size;
        }

        public ItemSlot(Point location)
        {
            Location = location;
            inventorySlotBackground = GameManager.Instance.InventorySlotBackground;
            Size = inventorySlotBackground.Bounds.Size;
        }

        public ItemSlot(Point location, Item item)
        {
            Location = location;
            Item = item;
            inventorySlotBackground = GameManager.Instance.InventorySlotBackground;
            Size = inventorySlotBackground.Bounds.Size;
        }

        public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(inventorySlotBackground, Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, GameManager.INVENTORY_LAYER);
            if (Item != null)
            {
                Item.Draw(sprite, Location.ToVector2() + new Vector2(inventorySlotBackground.Width / 2 - Item.Texture.Width / 2,
                    inventorySlotBackground.Height / 2 - Item.Texture.Height / 2)
                    ,GameManager.INVENTORY_LAYER * 0.1f);
            }
        }
    }
}

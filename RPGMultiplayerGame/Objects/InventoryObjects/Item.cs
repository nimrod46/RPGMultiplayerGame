using Microsoft.Xna.Framework;
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
    public abstract class Item
    {
        public Texture2D Texture { get; set; }
        public InventoryItemType ItemType
        {
            get => itemType; set
            {
                itemType = value;
                Texture = GameManager.Instance.GetItemByType(value);
            }
        }

        private InventoryItemType itemType;

        public Item(InventoryItemType itemType)
        {
            this.ItemType = itemType;
        }

        public virtual void Draw(SpriteBatch sprite, Vector2 location, float layer)
        {
            sprite.Draw(Texture, location, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layer);
        }
    }
}

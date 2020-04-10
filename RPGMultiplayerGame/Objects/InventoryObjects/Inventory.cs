using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;

namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public class Inventory : NetworkIdentity
    {
        public enum ItemType
        {
            CommonSword,
            CommonWond,
            BatClaw,
        }

        private readonly Texture2D inventorySlotBackground;
        public bool IsVisible { get; set; }
        private readonly KeyValuePair<Vector2, Item>[] inventoryItems;
        private readonly int columns;
        private readonly int rows;

        public Inventory(int columns, int rows)
        {
            this.columns = columns;
            this.rows = rows;
            inventoryItems = new KeyValuePair<Vector2, Item>[columns * rows];
            inventorySlotBackground = GameManager.Instance.InventorySlotBackground;
            IsVisible = false;
            int index = 0;
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    Vector2 location = new Vector2(i * inventorySlotBackground.Width, j * inventorySlotBackground.Height) + GameManager.Instance.GetMapSize().ToVector2() / 2
                    - new Vector2(columns * inventorySlotBackground.Width / 2, rows * inventorySlotBackground.Height / 2);
                    inventoryItems[index] = new KeyValuePair<Vector2, Item>(location, null);
                    index++;
                }
            }
        }

        public void Draw(SpriteBatch sprite)
        {
            if (IsVisible)
            {

                foreach (var item in inventoryItems)
                {
                    Item inventoryItem = item.Value;
                    sprite.Draw(inventorySlotBackground, item.Key, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, GameManager.INVENTORY_LAYER);
                    if (inventoryItem != null)
                    {
                        inventoryItem.Draw(sprite, item.Key + new Vector2(inventorySlotBackground.Width / 2 - inventoryItem.Texture.Width / 2,
                            inventorySlotBackground.Height / 2 - inventoryItem.Texture.Height / 2)
                            , GameManager.INVENTORY_LAYER * 0.1f);
                    }
                }
            }
        }

        public bool GetInventoryItemAtScreenLocation(Rectangle rect, out Item inventoryItem)
        {
            inventoryItem = null;
            for (int i = 0; i < inventoryItems.Count(); i++)
            {
                var item = inventoryItems[i];
                if(item.Value == null)
                {
                    continue;
                }
                Rectangle rectangle = new Rectangle(item.Key.ToPoint(), new Point(inventorySlotBackground.Width, inventorySlotBackground.Height));
                if (rectangle.Intersects(rect))
                {
                    inventoryItem = item.Value;
                    return true;
                }
            }
            return false;
        }

        public bool TryAddItem(Item inventoryItemToAdd)
        {
            if (inventoryItemToAdd is StackableItem)
            {
                if (HaveStackbleItem(inventoryItemToAdd.ItemType, out StackableItem stackableItem))
                {
                    stackableItem.Count += (inventoryItemToAdd as StackableItem).Count;
                    return true;
                }
            }
            for (int i = 0; i < inventoryItems.Count(); i++)
            {
                var item = inventoryItems[i];
                if (item.Value == null)
                {
                    inventoryItems[i] = new  KeyValuePair<Vector2, Item>(item.Key, inventoryItemToAdd);
                    return true;
                }
            }
            return false;
        }
        public bool HaveStackbleItem(ItemType itemType, out StackableItem stackableItem)
        {
            stackableItem = null;
            for (int i = 0; i < inventoryItems.Count(); i++)
            {
                    Item inventoryItem = this.inventoryItems[i].Value;
                if (inventoryItem is StackableItem && inventoryItem.ItemType == itemType)
                {
                    stackableItem = inventoryItem as StackableItem;
                    return true;
                }
            }
            return false;
        }
    }
}

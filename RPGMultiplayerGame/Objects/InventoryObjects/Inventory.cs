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

        public bool IsVisible { get; set; }
        private readonly ItemSlot[] inventoryItems;
        private readonly int columns;
        private readonly int rows;

        public Inventory(int columns, int rows)
        {
            this.columns = columns;
            this.rows = rows;
            inventoryItems = new ItemSlot[columns * rows];
            IsVisible = false;
            int index = 0;
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    ItemSlot inventoryItem = new ItemSlot();
                    Vector2 location = new Vector2(i * inventoryItem.Size.X, j * inventoryItem.Size.Y) + GameManager.Instance.GetMapSize().ToVector2() / 2
                    - new Vector2(columns * inventoryItem.Size.X / 2, rows * inventoryItem.Size.Y / 2);
                    inventoryItem.Location = location.ToPoint();
                    inventoryItems[index] = inventoryItem;
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
                    item.Draw(sprite);
                }
            }
        }

        public bool GetInventoryItemAtScreenLocation(Rectangle rect, out Item item)
        {
            item = null;
            for (int i = 0; i < inventoryItems.Count(); i++)
            {
                var inventoryItem = inventoryItems[i];
                if(inventoryItem.Item == null)
                {
                    continue;
                }
                Rectangle rectangle = new Rectangle(inventoryItem.Location, inventoryItem.Size);
                if (rectangle.Intersects(rect))
                {
                    item = inventoryItem.Item;
                    return true;
                }
            }
            return false;
        }

        public bool TryAddItem(Item itemToAdd)
        {
            if (itemToAdd is StackableItem)
            {
                if (HaveStackbleItem(itemToAdd.ItemType, out StackableItem stackableItem))
                {
                    stackableItem.Count += (itemToAdd as StackableItem).Count;
                    return true;
                }
            }
            for (int i = 0; i < inventoryItems.Count(); i++)
            {
                var inventoryItem = inventoryItems[i];
                if (inventoryItem.Item == null)
                {
                    inventoryItem.Item = itemToAdd;
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
                Item item = inventoryItems[i].Item;
                if (item is StackableItem && item.ItemType == itemType)
                {
                    stackableItem = item as StackableItem;
                    return true;
                }
            }
            return false;
        }
    }
}

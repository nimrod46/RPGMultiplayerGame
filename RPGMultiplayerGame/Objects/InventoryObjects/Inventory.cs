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
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public class Inventory
    {
        public enum ItemType
        {
            CommonSword,
            CommonWond,
            BatClaw,
            CommonHealthPotion,
        }

        public bool IsVisible { get; set; }
        private readonly ItemSlot[] inventoryItems;
        private readonly Point origin;
        private readonly int columns;
        private readonly int rows;

        public Inventory(Point origin, int columns, int rows)
        {
            this.origin = origin;
            this.columns = columns;
            this.rows = rows;
            inventoryItems = new ItemSlot[columns * rows];
            IsVisible = true;
            int index = 0;
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    ItemSlot inventoryItem = new ItemSlot();
                    Vector2 location = new Vector2(i * inventoryItem.Size.X, j * inventoryItem.Size.Y) + origin.ToVector2()
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

        public void UsePotionAtSlot(int slot, Entity entity)
        {
            if(TryGetItemInSlot(slot, out Item item))
            {
                if(item is Potion potion)
                {
                    potion.UseOn(entity);
                    if(potion.IsDone())
                    {
                        TryRemoveItem(potion);
                    }
                }
            }
        }

        public bool TryGetInventoryItemAtScreenLocation(Rectangle rect, out Item item)
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
                    stackableItem.Add(itemToAdd as StackableItem);
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

        internal bool TryGetItemInSlot(int slot, out Item item)
        {
            ItemSlot itemSlot = inventoryItems[slot - 1];
            item = itemSlot.Item;
            return item != null;
        }

        internal bool TryRemoveItem(Item item)
        {
            for (int i = inventoryItems.Length - 1; i >= 0; i--)
            {
                if(inventoryItems[i].Item != null && inventoryItems[i].Item.ItemType == item.ItemType)
                {
                    inventoryItems[i].Item = null;
                    return true;
                }
            }
            return false;
        }
    }
}

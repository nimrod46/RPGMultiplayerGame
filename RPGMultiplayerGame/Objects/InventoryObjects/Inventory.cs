using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public class Inventory : NetworkIdentity
    {
        public enum InventoryItemType
        {
            None,
            CommonSword,
            CommonWond,
            BatClaw,
        }

        private Texture2D inventorySlotBackground;
        public bool IsVisible { get; set; }
        private readonly KeyValuePair<Vector2, Item>[] inventoryItems = new KeyValuePair<Vector2, Item>[12];

        public Inventory()
        {
            inventorySlotBackground = GameManager.Instance.InventorySlotBackground;
            IsVisible = false;
                int j = 0;
            for (int i = 0; i < inventoryItems.Count(); i++)
            {
                if (i % (this.inventoryItems.Count() / 2) == 0)
                {
                    j = i / (this.inventoryItems.Count() / 2);
                }
                Vector2 location = new Vector2((i - j * (this.inventoryItems.Count() / 2)) * inventorySlotBackground.Width, j * inventorySlotBackground.Height) + GameManager.Instance.GetMapSize().ToVector2() / 2 -
                       new Vector2(this.inventoryItems.Count() / 2 * inventorySlotBackground.Width / 2, this.inventoryItems.Count() / 2 * inventorySlotBackground.Height / 2);
                inventoryItems[i] = new KeyValuePair<Vector2, Item>(location, null);
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
        public bool HaveStackbleItem(InventoryItemType itemType, out StackableItem stackableItem)
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

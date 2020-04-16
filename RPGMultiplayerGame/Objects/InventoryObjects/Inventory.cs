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
using RPGMultiplayerGame.Objects.Other;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public class Inventory<T> : IGameDrawable, IGameUpdateable where T : GameItem
    {
        public delegate void ItemClickedEventHandler(T item);
        public event ItemClickedEventHandler OnItemClickedEvent;

        public bool IsVisible
        {
            get => isVisible; set
            {
                isVisible = value;
                GameManager.Instance.IsMouseVisible = isVisible;
            }
        }

        private readonly ItemSlot<T>[] inventoryItems;
        private bool isVisible;

        public Inventory(Point origin, OriginLocationType originType, int columns, int rows)
        {
            ItemSlot<T> inventoryItem = new ItemSlot<T>();
            switch (originType)
            {
                case OriginLocationType.Centered:
                    origin = (origin.ToVector2() - new Vector2(inventoryItem.Size.X * columns / 2, inventoryItem.Size.Y * rows / 2)).ToPoint();
                    break;
                case OriginLocationType.ButtomLeft:
                    origin = (origin.ToVector2() - new Vector2(0, inventoryItem.Size.Y * rows)).ToPoint();
                    break;
                case OriginLocationType.ButtomCentered:
                    origin = (origin.ToVector2() - new Vector2(inventoryItem.Size.X * columns / 2, inventoryItem.Size.Y * rows)).ToPoint();
                    break;
            }
            inventoryItems = new ItemSlot<T>[columns * rows];
            IsVisible = true;
            int index = 0;
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    inventoryItem = new ItemSlot<T>();
                    Vector2 location = new Vector2(i * inventoryItem.Size.X, j * inventoryItem.Size.Y) + origin.ToVector2();
                    inventoryItem.Location = location.ToPoint();
                    inventoryItem.Item = ItemFactory.GetEmptyItem<T>();
                    inventoryItems[index] = inventoryItem;
                    index++;
                }
            }
            GameManager.Instance.AddGraphicObject(this);
            GameManager.Instance.AddUpdateObject(this);
            ServerManager.Instance.AddServerGameObject(this);
        }

        public void Update(GameTime gameTime)
        {
            if (IsVisible)
            {
                if (InputManager.Instance.GetMouseLeftButtonPressed())
                {
                    if (TryGetInventoryItemAtScreenLocation(InputManager.Instance.MouseBounds(), out T item))
                    {
                        OnItemClickedEvent?.Invoke(item);
                    }

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
            if (TryGetItemInSlot(slot, out T item))
            {
                if (item is Potion potion)
                {
                    potion.UseOn(entity);
                }
            }
        }

        public void PutItemInSlot(int slot, T item)
        {
            inventoryItems[slot - 1].Item = item;
        }

        public bool TryGetInventoryItemAtScreenLocation(Rectangle rect, out T item)
        {
            item = null;
            for (int i = 0; i < inventoryItems.Count(); i++)
            {
                var inventoryItem = inventoryItems[i];
                if (!inventoryItem.Item.IsExists())
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

        public bool TryAddItem(T itemToAdd)
        {
                    Console.WriteLine("ADDED {0} POTIONS", itemToAdd);
            if (itemToAdd is StackableGameItem)
            {
                if (HaveStackbleItem(itemToAdd.ItemType, out StackableGameItem stackableItem))
                {
                    stackableItem.Add(itemToAdd as StackableGameItem);
                    return true;
                }
            }
            for (int i = 0; i < inventoryItems.Count(); i++)
            {
                var inventoryItem = inventoryItems[i];
                if (!inventoryItem.Item.IsExists())
                {
                    inventoryItem.Item = itemToAdd;
                    return true;
                }
            }
            return false;
        }
        public bool HaveStackbleItem(ItemType itemType, out StackableGameItem stackableItem)
        {
            stackableItem = null;
            for (int i = 0; i < inventoryItems.Count(); i++)
            {
                GameItem item = inventoryItems[i].Item;
                if (item is StackableGameItem && item.ItemType == itemType)
                {
                    stackableItem = item as StackableGameItem;
                    return true;
                }
            }
            return false;
        }

        internal bool TryGetItemInSlot(int slot, out T item)
        {
            ItemSlot<T> itemSlot = inventoryItems[slot - 1];
            item = itemSlot.Item;
            return item != null;
        }

        internal bool TryRemoveItem(GameItem item)
        {
            for (int i = inventoryItems.Length - 1; i >= 0; i--)
            {
                if (inventoryItems[i].Item.IsExists() && inventoryItems[i].Item.ItemType == item.ItemType)
                {
                    inventoryItems[i].Item = ItemFactory.GetEmptyItem<T>();
                    return true;
                }
            }
            return false;
        }
    }
}

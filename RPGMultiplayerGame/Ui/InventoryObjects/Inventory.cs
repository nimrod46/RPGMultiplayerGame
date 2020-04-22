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
using RPGMultiplayerGame.Ui;
using static RPGMultiplayerGame.Managers.GameManager;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public class Inventory<T> : UiComponent, IGameUpdateable where T : GameItem
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

        private readonly ItemSlotUi<T>[] inventoryItems;
        private bool isVisible;

        public Inventory(Func<Point, Vector2> origin, PositionType positionType, int columns, int rows, bool defaultVisibility) : base(origin, positionType, GameManager.GUI_LAYER)
        {
            isVisible = defaultVisibility;
            ItemSlotUi<T> inventoryItem = new ItemSlotUi<T>(origin, positionType);
            Size = inventoryItem.Size * new Vector2(columns, rows);
            inventoryItems = new ItemSlotUi<T>[columns * rows];
            int index = 0;
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    int x = i; int y = j;
                    inventoryItem = new ItemSlotUi<T>((windowSize) =>
                    {
                        Console.WriteLine(i);
                        return Position + new Vector2(x * inventoryItem.Size.X, y * inventoryItem.Size.Y);
                    }
                    , PositionType.TopLeft, ItemFactory.GetEmptyItem<T>());
                    inventoryItems[index] = inventoryItem;
                    index++;
                }
            }
            GameManager.Instance.AddGraphicObject(this);
            GameManager.Instance.AddUpdateObject(this);
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

        public override void Draw(SpriteBatch sprite)
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
                Rectangle rectangle = new Rectangle(inventoryItem.Position.ToPoint(), inventoryItem.Size.ToPoint());
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
            ItemSlotUi<T> itemSlot = inventoryItems[slot - 1];
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

using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Ui;
using System;
using System.Linq;

namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public class Inventory<T> : UiComponent, IGameUpdateable where T : GameItem
    {
        public delegate void ItemClickedEventHandler(Inventory<T> inventory, ItemSlotUi<T> item);
        public event ItemClickedEventHandler OnItemLeftClickedEvent;
        public event ItemClickedEventHandler OnItemRightClickedEvent;

        public override bool IsVisible
        {
            get => base.IsVisible; set
            {
                base.IsVisible = value;
                foreach (var item in inventoryItems)
                {
                    item.IsVisible = value;
                }
            }
        }

        public bool IsIntractable
        {
            get => isIntractable; set
            {
                isIntractable = value;
                GameManager.Instance.IsMouseInteractable = isIntractable;
            }
        }
        public bool IsDestroyed { get; set; }
        private readonly ItemSlotUi<T>[] inventoryItems;
        private bool isIntractable;

        public Inventory(Func<Point, Vector2> origin, PositionType positionType, bool defaultVisibility, int columns, int rows) : base(origin, positionType, defaultVisibility, UiManager.GUI_LAYER)
        {
            isIntractable = false;
            IsDestroyed = false;
            ItemSlotUi<T> inventoryItem = new ItemSlotUi<T>(origin, positionType, false, Activator.CreateInstance<T>());
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
                        return Position + new Vector2(x * inventoryItem.Size.X, y * inventoryItem.Size.Y);
                    }
                    , PositionType.TopLeft, defaultVisibility, Activator.CreateInstance<T>());
                    inventoryItems[index] = inventoryItem;
                    index++;
                }
            }
            GameManager.Instance.AddUpdateObject(this);
        }

        public void Intracte()
        {
           
        }

        private ItemSlotUi<T> lastItemSlot;

        public void Update(GameTime gameTime)
        {
            if (IsIntractable)
            {
                if (TryGetInventoryItemAtScreenLocation(InputManager.Instance.MouseBounds(), out ItemSlotUi<T> itemSlot))
                {
                    if (InputManager.Instance.GetMouseLeftButtonPressed())
                    {
                        OnItemLeftClickedEvent?.Invoke(this, itemSlot);
                    }
                    else if(InputManager.Instance.GetMouseRightButtonPressed())
                    {
                        OnItemRightClickedEvent?.Invoke(this, itemSlot);
                    }
                    if (lastItemSlot != null && lastItemSlot != itemSlot)
                    {
                        lastItemSlot.HideDescription();
                    }
                    itemSlot.ShowDescription();
                    lastItemSlot = itemSlot;
                }
                else if (lastItemSlot != null)
                {
                    lastItemSlot.HideDescription();
                    lastItemSlot = null;
                }

            }
            else if (lastItemSlot != null)
            {
                lastItemSlot.HideDescription();
                lastItemSlot = null;
            }

            if (!GameManager.Instance.IsMouseInteractable)
            {
                if (lastItemSlot != null)
                {
                    lastItemSlot.HideDescription();
                    lastItemSlot = null;
                }
            }
        }

        public void DropItem(ItemSlotUi<T> itemSlot, Vector2 location)
        {
            itemSlot.Item.SetAsMapItem(location);
            TryRemoveItem(itemSlot.Item);
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

        public bool TryGetInventoryItemAtScreenLocation(Rectangle rect, out ItemSlotUi<T> item)
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
                    item = inventoryItem;
                    return true;
                }
            }
            return false;
        }

        public bool TryAddItem(T itemToAdd)
        {
            if (itemToAdd is StackableGameItem)
            {
                if (HaveStackbleItem(itemToAdd.SyncItemType, out StackableGameItem stackableItem))
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
                    inventoryItem.Item.Delete();
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
                if (item is StackableGameItem && item.SyncItemType == itemType)
                {
                    stackableItem = item as StackableGameItem;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetItemInSlot(int slot, out T item)
        {
            ItemSlotUi<T> itemSlot = inventoryItems[slot - 1];
            item = itemSlot.Item;
            return item.IsExists();
        }

        public int GetItemSlot(T item)
        {
            return inventoryItems.ToList().FindIndex(i => i.Item == item) + 1;
        }

        public bool TryRemoveItem(GameItem item)
        {
            for (int i = inventoryItems.Length - 1; i >= 0; i--)
            {
                if (inventoryItems[i].Item.IsExists() && inventoryItems[i].Item == item)
                {
                    inventoryItems[i].Item = Activator.CreateInstance<T>();
                    return true;
                }
            }
            return false;
        }
    }
}

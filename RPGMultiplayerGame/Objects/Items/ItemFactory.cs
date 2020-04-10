using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.Items
{
    public static class ItemFactory
    {
        public static Item EmptyItem = new EmptyItem();
        public static T GetItem<T>(ItemType itemType) where T : Item
        {
            switch (itemType)
            {
                case ItemType.CommonSword:
                    return new CommonSword() as T;
                case ItemType.CommonWond:
                    return new CommonWond() as T;
                case ItemType.BatClaw:
                    return new BatClaw() as T;
            }
            return null;
        }

        public static T GetItem<T>(ItemType itemType, int count) where T : Item
        {
            switch (itemType)
            {
                case ItemType.CommonHealthPotion:
                    return new CommonHealthPotion(count) as T;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public static class ItemFactory
    {
        public static T GetInventoryItem<T>(ItemType itemType) where T : Item
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
    }
}

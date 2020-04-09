using RPGMultiplayerGame.Objects.InventoryObjects.InventoryItems.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.InventoryObjects.InventoryItems
{
    public static class InventoryItemFactory
    {
        public static T GetInventoryItem<T>(InventoryItemType itemType) where T : InventoryItem
        {
            switch (itemType)
            {
                case InventoryItemType.None:
                    return null;
                case InventoryItemType.CommonSword:
                    return new CommonSword() as T;
                case InventoryItemType.CommonWond:
                    return new CommonWond() as T;
                case InventoryItemType.BatClaw:
                    return new BatClaw() as T;
            }
            return null;
        }
    }
}

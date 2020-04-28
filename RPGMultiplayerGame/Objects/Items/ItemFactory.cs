using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items
{
    public enum ItemType
    {
        None,
        CommonSword,
        CommonWond,
        BatClaw,
        CommonHealthPotion,
    }

    //public static class ItemFactory
    //{
    //    public static GameItem EmptyItem = new EmptyItem();

    //    public static T GetEmptyItem<T>() where T : GameItem
    //    {
    //        return Activator.CreateInstance<T>();
    //    }

    //    public static T GetItem<T>(ItemType itemType) where T : GameItem
    //    {
    //        return itemType switch
    //        {
    //            ItemType.CommonSword => new CommonSword() as T,
    //            ItemType.CommonWond => new CommonWond() as T,
    //            ItemType.BatClaw => new BatClaw() as T,
    //            _ => throw new Exception("Cannot create insatance of " + itemType),
    //        };
    //    }

    //    public static T GetItem<T>(ItemType itemType, int count) where T : GameItem
    //    {
    //        return itemType switch
    //        {
    //            ItemType.CommonHealthPotion => new CommonHealthPotion(count) as T,
    //            _ => throw new Exception("Cannot create insatance of " + itemType),
    //        };
    //    }

    //    public static void GivePlayerItemByItem<T>(Player player, T item) where T : GameItem
    //    {
    //        if(item is StackableGameItem stackableGameItem)
    //        {
    //            player.AddItemToInventory(item.SyncItemType, stackableGameItem.SyncCount);

    //        }
    //        else if (item is GameItem)
    //        {
    //            player.AddItemToInventory(item.SyncItemType);
    //        }
    //    }
    //}
}

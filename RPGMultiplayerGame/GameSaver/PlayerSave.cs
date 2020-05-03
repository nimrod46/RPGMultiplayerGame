using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGMultiplayerGame.GameSaver
{
    public class PlayerSave : IObjectSave<Player>
    {
        public readonly List<GameItem> gameItems;
        public readonly List<Quest> quests;
        public long gold;
        public Vector2 location;

        public PlayerSave()
        {
            gameItems = new List<GameItem>();
            quests = new List<Quest>();
        }

        public void ResetData()
        {
            gameItems.Clear();
            quests.Clear();
            gold = 0;
        }

        public void SaveObjectData(Player player)
        {
            gameItems.AddRange(ServerManager.Instance.CopyIdentities().Where(i => i is GameItem gameItem && gameItem.OwnerId == player.OwnerId).Cast<GameItem>().Select(i => (GameItem) Activator.CreateInstance(i.GetType())));
            quests.AddRange(ServerManager.Instance.CopyIdentities().Where(i => i is Quest quest && quest.OwnerId == player.OwnerId).Cast<Quest>());
            gold = player.SyncGold;
            location = new Vector2(player.SyncX, player.SyncY);
        }

        public void LoadObjectData(Player player)
        {
            foreach (var item in gameItems)
            {
                ServerManager.Instance.GivePlayerGameItem(player, (dynamic) item);
            }
            foreach (var quest in quests)
            {
                ServerManager.Instance.AddQuest(player, (dynamic) quest);
            }

            player.SyncGold = gold;
            player.SyncX = location.X;
            player.SyncY = location.Y;
        }
    }
}

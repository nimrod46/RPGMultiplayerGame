using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RPGMultiplayerGame
{
    public class GameSave
    {
        [Serializable]
        [XmlType(TypeName = "KeyValuePair")]
        public struct SerializableKeyValuePair<K, V>
        {
            public K Key
            { get; set; }

            public V Value
            { get; set; }
            public SerializableKeyValuePair(K key, V value)
            {
                Key = key;
                Value = value;
            }
        }

        public List<SerializableKeyValuePair<string, List<GameItem>>> itemsByPlayersName;

        public GameSave()
        {
            this.itemsByPlayersName = new List<SerializableKeyValuePair<string, List<GameItem>>>();
        }

        public void SavePlayerData(Player player, GameItem[] gameItems)
        {
            if (!itemsByPlayersName.Where(s => s.Key == player.GetName()).Any())
            {
                itemsByPlayersName.Add(new SerializableKeyValuePair<string, List<GameItem>>(player.GetName(), new List<GameItem>()));
            }
            else
            {
                itemsByPlayersName.Where(s => s.Key == player.GetName()).First().Value.Clear();
            }
            itemsByPlayersName.Where(s => s.Key == player.GetName()).First().Value.AddRange(gameItems);
        }

        internal bool LoadPlayerSave(Player player)
        {
            lock (itemsByPlayersName)
            {
                if (itemsByPlayersName.Where(s => s.Key == player.GetName()).Any())
                {
                    foreach (var item in itemsByPlayersName.Where(s => s.Key == player.GetName()).First().Value)
                    {
                        ServerManager.Instance.GivePlayerGameItem(player, item);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}

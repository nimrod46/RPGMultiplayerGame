using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RPGMultiplayerGame.GameSaver
{
    public class GameSave
    {
        [Serializable]
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

        public List<SerializableKeyValuePair<string, PlayerSave>> playersSaveByName;
        public List<SerializableKeyValuePair<string, NpcSave>> npcsSaveByName;

        public GameSave()
        {
            playersSaveByName = new List<SerializableKeyValuePair<string, PlayerSave>>();
            npcsSaveByName = new List<SerializableKeyValuePair<string, NpcSave>>();
        }

        public void SaveObjectData(Human gameObject)
        {
            if (gameObject is Player player)
            {
                SaveDataByList(player, playersSaveByName);
            }
            if (gameObject is Npc npc)
            {
                SaveDataByList(npc, npcsSaveByName);
            }
        }

        private void SaveDataByList<T, V>(T gameObject, List<SerializableKeyValuePair<string, V>> objectsByName) where T : Human where V : IObjectSave<T>
        {
            if (!objectsByName.Where(s => s.Key == gameObject.GetName()).Any())
            {
                objectsByName.Add(new SerializableKeyValuePair<string, V>(gameObject.GetName(), Activator.CreateInstance<V>()));
            }
            else
            {
                objectsByName.Where(s => s.Key == gameObject.GetName()).First().Value.ResetData();
            }
            objectsByName.Where(s => s.Key == gameObject.GetName()).First().Value.SaveObjectData(gameObject);
        }

        internal bool LoadObjectSave(Human gameObject)
        {
            if (gameObject is Player player)
            {
                return LoadDataByList(player, playersSaveByName);
            }
            if (gameObject is Npc npc)
            {
                return LoadDataByList(npc, npcsSaveByName);
            }
            return false;
        }

        private bool LoadDataByList<T, V>(T gameObject, List<SerializableKeyValuePair<string, V>> objectsByName) where T : Human where V : IObjectSave<T>
        {
            if (objectsByName.Where(s => s.Key == gameObject.GetName()).Any())
            {
                objectsByName.Where(s => s.Key == gameObject.GetName()).First().Value.LoadObjectData(gameObject);
                return true;
            }
            return false;
        }
    }
}

using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGMultiplayerGame.GameSaver
{
    public class GameSave
    {
        [Serializable]
        public struct SerializableKeyValuePair<TKey, TValue>
        {
            public TKey Key
            { get; set; }

            public TValue Value
            { get; set; }
            public SerializableKeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        private readonly List<SerializableKeyValuePair<string, PlayerSave>> playersSaveByName;
        private readonly List<SerializableKeyValuePair<string, NpcSave>> npcsSaveByName;

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

        private static bool LoadDataByList<TKey, TValue>(TKey gameObject, List<SerializableKeyValuePair<string, TValue>> objectsByName) where TKey : Human where TValue : IObjectSave<TKey>
        {
            if (objectsByName.Any(s => s.Key == gameObject.GetName()))
            {
                objectsByName.First(s => s.Key == gameObject.GetName()).Value.LoadObjectData(gameObject);
                return true;
            }
            return false;
        }

        private static void SaveDataByList<TKey, TValue>(TKey gameObject, List<SerializableKeyValuePair<string, TValue>> objectsByName) where TKey : Human where TValue : IObjectSave<TKey>
        {
            if (objectsByName.All(s => s.Key != gameObject.GetName()))
            {
                objectsByName.Add(new SerializableKeyValuePair<string, TValue>(gameObject.GetName(), Activator.CreateInstance<TValue>()));
            }
            else
            {
                objectsByName.First(s => s.Key == gameObject.GetName()).Value.ResetData();
            }
            objectsByName.First(s => s.Key == gameObject.GetName()).Value.SaveObjectData(gameObject);
        }
    }
}

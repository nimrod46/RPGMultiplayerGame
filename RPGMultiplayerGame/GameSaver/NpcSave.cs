using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.GameSaver.GameSave;

namespace RPGMultiplayerGame.GameSaver
{
    public class NpcSave : IObjectSave<Npc>
    {
        public readonly List<SerializableKeyValuePair<string, int>> playersProgress;

        public NpcSave()
        {
            playersProgress = new List<SerializableKeyValuePair<string, int>>();
        }

        public void ResetData()
        {
            playersProgress.Clear();
        }

        public void SaveObjectData(Npc obj)
        {
            playersProgress.AddRange(obj.CopyPlayerProgress().Select(e => new SerializableKeyValuePair<string, int>(e.Key, e.Value)));
        }

        public void LoadObjectData(Npc obj)
        {
            obj.SetPlayerProgress(playersProgress.Select(e => new KeyValuePair<string, int>(e.Key, e.Value)).ToDictionary(k => k.Key, v => v.Value));
        }
    }
}

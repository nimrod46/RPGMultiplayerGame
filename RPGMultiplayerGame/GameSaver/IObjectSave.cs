using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.GameSaver
{
    interface IObjectSave<T> where T : Human
    {
        public void ResetData();

        public void SaveObjectData(T obj);

        public void LoadObjectData(T obj);
    }
}

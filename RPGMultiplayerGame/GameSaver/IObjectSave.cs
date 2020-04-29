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

using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.GameSaver
{
    internal interface IObjectSave<in T> where T : Human
    {
        public void ResetData();

        public void SaveObjectData(T obj);

        public void LoadObjectData(T obj);
    }
}

using Map;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.MapObjects;

namespace RPGMultiplayerGame.MapObjects
{
    public class SpawnPoint : MarkObject
    {
        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.spawnPoint = this;
            NetworkManager.Instance.UpdateSpawnLocation(this);
        }
    }
}
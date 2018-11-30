using Map;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.MapObjects;

namespace RPGMultiplayerGame.MapObjects
{
    public class SpawnMark : MarkObject
    {
        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.spawnPoint = this;
            NetworkManager.Instance.UpdateSpawnLocation(this);
        }
    }
}
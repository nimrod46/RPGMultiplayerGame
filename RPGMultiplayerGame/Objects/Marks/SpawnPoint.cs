using Networking;
using RPGMultiplayerGame.Objects.MapObjects;

namespace RPGMultiplayerGame.MapObjects
{
    public class SpawnPoint : MarkObject
    {
        public override void OnDestroyed(NetworkIdentity identity)
        {
            throw new System.NotImplementedException();
        }
    }
}
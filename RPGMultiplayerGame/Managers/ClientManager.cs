using Networking;

namespace RPGMultiplayerGame.Managers
{
    public class ClientManager : NetworkManager<ClientBehavior>
    {
        public static ClientManager instance;
        public static ClientManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientManager();
                }
                return instance;
            }
        }
        protected ClientManager()
        {
        }

        public void Start()
        {
            NetBehavior.OnLocalIdentityInitialize += GameManager.Instance.OnIdentityInitialize;
            NetBehavior.OnRemoteIdentityInitialize += GameManager.Instance.OnIdentityInitialize;
            NetBehavior.Synchronize();
        }



        public bool Connect()
        {
            NetBehavior = lobby.Connect();
            if (NetBehavior != null)
            {
                return true;
            }
            return false;
        }
    }
}

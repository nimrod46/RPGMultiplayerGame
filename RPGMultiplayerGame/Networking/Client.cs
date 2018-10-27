using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
namespace RPGMultiplayerGame.Networking
{
    public class Client : NetworkIdentity
    {
        public NetworkBehavior NetBehavior { get; set; }

        public override void OnLocalPlayerInitialize()
        {
            base.OnLocalPlayerInitialize();
            NetBehavior = NetworkManager.Instance.NetBehavior;

            if (!isServer)
            {
            }
        }
    }
}

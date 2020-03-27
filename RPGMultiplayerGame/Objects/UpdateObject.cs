using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
namespace RPGMultiplayerGame.Objects
{
    public abstract class UpdateObject : GameObject
    {
        public UpdateObject()
        {
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.AddUpdateObject(this);
            if (isServerAuthority)
            {
                ServerManager.Instance.AddServerGameObject(this);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            GameManager.Instance.RemoveUpdateObject(this);
            base.OnDestroyed(identity);
        }
    }
}

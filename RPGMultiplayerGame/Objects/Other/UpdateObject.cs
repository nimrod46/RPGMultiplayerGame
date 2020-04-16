using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
namespace RPGMultiplayerGame.Objects.Other
{
    public abstract class UpdateObject : GameObject
    {
        public UpdateObject()
        {
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            if (isInServer && hasAuthority)
            {
                ServerManager.Instance.AddServerGameObject(this);
            }
            else
            {
                GameManager.Instance.AddUpdateObject(this);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            base.OnDestroyed(identity);
            if (isInServer && hasAuthority)
            {
                ServerManager.Instance.RemoveServerGameObject(this);
            }
            else
            {
                GameManager.Instance.RemoveUpdateObject(this);
            }
        }
    }
}

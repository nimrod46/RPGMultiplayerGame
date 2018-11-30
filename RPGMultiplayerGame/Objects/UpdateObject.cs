using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects
{
    public abstract class UpdateObject : GraphicObject
    {
        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.AddUpdateObject(this);
            layer -= 0.01f;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (hasAuthority)
                Location = new Vector2(SyncX, SyncY);
        }

        public override void OnDestroyed()
        {
            GameManager.Instance.RemoveUpdateObject(this);
            base.OnDestroyed();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Other;

namespace RPGMultiplayerGame.Networking
{
    abstract class GameObject : NetworkIdentity
    {
        public Vector2 Location { get; set; }
        [SyncVar(hook = "OnXSet")]
        public float SyncX { get; set; }
        [SyncVar(hook = "OnYSet")]
        public float SyncY { get; set; }
        protected Texture2D texture;

        public override void OnNetworkInitialize()
        {
            MapManager.Instance.AddGameObject(this);
            GameManager.Instance.AddGameObject(this);
        }

        public void OnXSet()
        {
            Location = new Vector2(SyncX, Location.Y);
        }

        public void OnYSet()
        {
            Location = new Vector2(Location.X, SyncY);
        }

        public void Draw(SpriteBatch sprite)
        {
            if (texture != null)
            {
                sprite.Draw(texture, Location, Color.White);
            }
        }

        public virtual void Update(GameTime gameTime)
        {

        }
    }
}

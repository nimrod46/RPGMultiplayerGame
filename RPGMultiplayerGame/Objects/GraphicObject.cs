﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects
{
    public abstract class GraphicObject : GameObject
    {
        
        protected Texture2D texture;
        protected float layer = 1;
        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.AddGraphicObject(this);
            controling = hasAuthority;
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            if (texture != null)
            {
                sprite.Draw(texture, Location, null, Color.White, 0,Vector2.Zero, 1, SpriteEffects.None, layer);
            }
            else
            {
                Console.Error.WriteLine("Cannot draw object object: " + id + " " + GetType());
            }
        }

        public override void OnDestroyed()
        {
            GameManager.Instance.RemoveGraphicObject(this);
            base.OnDestroyed();
        }
    }
}
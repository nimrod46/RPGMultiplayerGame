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
    public abstract class GameObject : GraphicObject
    {
        public override void OnNetworkInitialize()
        {
            GameManager.Instance.AddObject(this);
        }

        public override void OnDestroyed()
        {
            GameManager.Instance.AddObject(this);
        }
    }
}

﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.VisualEffects
{
    public class VisualEffect : AnimatedObject
    {

        public enum VisualEffectAnimation
        {
            Idle
        }

        public GraphicObject SyncParent { get; set; }

        public VisualEffect(Dictionary<int, List<GameTexture>> animationsByType) : base(animationsByType)
        {
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            UpdateLocation();
        }

        public void UpdateLocation()
        {
            Location = SyncParent.Location;
            Location = Location + SyncParent.GetDrawCenter() - GetDrawCenter();
        }
    }
}

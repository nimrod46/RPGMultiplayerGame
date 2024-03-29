﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.VisualEffects
{
    public class StormVisualEffect : VisualEffect
    {
        public StormVisualEffect() : base(GraphicManager.Instance.AnimationsByVisualEffects[VisualEffectId.WindStorm])
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Layer = SyncParent.Layer + 0.01f;
            UpdateLocation();
        }

        public override void Draw(SpriteBatch sprite)
        {
            UpdateLocation();
            base.Draw(sprite);
        }
    }
}

using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.VisualEffects
{
    public class ExplotionVisualEffect : VisualEffect
    {
        public ExplotionVisualEffect() : base(GraphicManager.Instance.AnimationsByVisualEffect[VisualEffectId.Explotion])
        {
            scale *= 0.5f;
            SyncSpeed *= 2;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            shouldLoopAnimation = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Layer = SyncParent.Layer + 0.01f;
        }
    }
}

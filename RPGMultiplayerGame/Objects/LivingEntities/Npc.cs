using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    class Npc : Human
    {
        public Dictionary<double, List<Vector2>> path = new Dictionary<double, List<Vector2>>();
        private double currentPointTime = 0;
        public Npc(EntityID entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, nameFont)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!isServerAuthority)
            {
                return;
            }
            currentPointTime += gameTime.ElapsedGameTime.TotalSeconds;

        }
    }
}

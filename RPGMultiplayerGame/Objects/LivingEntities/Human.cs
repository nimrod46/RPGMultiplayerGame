using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public abstract class Human : Entity
    {
        [SyncVar(hook = "OnNameSet")]
        protected string syncName;

        private Vector2 nameFontOffset;
        private readonly SpriteFont nameFont;
        private Vector2 nameFontSize = Vector2.Zero;

        public Human(EntityID entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth)
        {
            this.nameFont = nameFont;
            BaseSize = (animationsByType[(int) EntityAnimation.IdleDown][0].Texture.Bounds.Size.ToVector2() * scale).ToPoint();
        }

        protected override void UpdateDrawOffset()
        {
            base.UpdateDrawOffset();
            nameFontOffset = new Vector2(BaseSize.X / 2 - nameFontSize.X / 2, -healthBarBackground.Height - 2 - nameFontSize.Y);
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            if (!isHidenCompletely)
            {
                sprite.DrawString(nameFont, syncName, Location + nameFontOffset, Color.Black, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, textLyer);
            }
        }

        protected void SetName(string name)
        {
            syncName = name;
        }

        public void OnNameSet()
        {
            nameFontSize = nameFont.MeasureString(syncName);
            UpdateDrawOffset();
        }

        public string GetName()
        {
            return syncName;
        }
    }
}

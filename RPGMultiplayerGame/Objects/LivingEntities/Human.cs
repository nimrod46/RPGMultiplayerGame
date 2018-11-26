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
    abstract class Human : Entity
    {
        [SyncVar(hook = "OnNameSet")]
        protected string syncName;
        [SyncVar(hook = "OnHealthSet")]
        protected float syncHealth;

        private Vector2 healthBarOffset;
        private Vector2 nameFontOffset;
        private Vector2 healthBarSize;
        private readonly Texture2D healthBar;
        private readonly Texture2D healthBarBackground;
        private readonly float maxHealth;
        private readonly SpriteFont nameFont;
        private Vector2 nameFontSize = Vector2.Zero;

        public Human(EntityID entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY)
        {
            this.maxHealth = maxHealth;
            this.nameFont = nameFont;
            syncHealth = maxHealth;
            healthBar = GameManager.Instance.HealthBar;
            healthBarBackground = GameManager.Instance.HealthBarBackground;
        }

        public override void OnNetworkInitialize()
        {
            if (!hasFieldsBeenInitialized)
            {
                syncName = "null";
                OnNameSet();
            }
            OnNameSet();
            base.OnNetworkInitialize();
            UpdateDrawOffset();
            healthBarSize = new Vector2(healthBar.Width, healthBar.Height);
        }

        public void OnHealthSet()
        {
            healthBarSize.X = syncHealth * healthBar.Width / maxHealth;
        }

        public override void OnAnimationIndexSet()
        {
            base.OnAnimationIndexSet();
            UpdateDrawOffset();
        }

        protected void UpdateDrawOffset()
        {
            healthBarOffset = new Vector2(size.X / 2 - healthBarBackground.Width / 2, -healthBarBackground.Height - 2);
            nameFontOffset = new Vector2(size.X / 2 - nameFontSize.X / 2, -healthBarBackground.Height - 2 - nameFontSize.Y);
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            if (hasInitialized)
            {
                if (syncName != null)
                {
                    sprite.DrawString(nameFont, syncName, Location + nameFontOffset, Color.Black, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer);
                }
                sprite.Draw(healthBarBackground, Location + healthBarOffset, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + 0.001f);
                sprite.Draw(healthBar, Location + healthBarOffset, new Rectangle(0,0, (int) healthBarSize.X, (int) healthBarSize.Y), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer);
            }
        }

        public void SetName(string name)
        {
            this.syncName = name;
        }

        public void OnNameSet()
        {
            nameFontSize = nameFont.MeasureString(syncName);
        }
    }
}

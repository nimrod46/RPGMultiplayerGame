using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public abstract class Human : PathEntity
    {
        protected string SyncName
        {
            get => syncName; set
            {
                syncName = value;
                InvokeSyncVarNetworkly(nameof(SyncName), value);
                OnNameSet();
            }
        }

        public Color NameColor { get; set; }

        protected readonly SpriteFont nameFont;
        protected Vector2 nameOffset;
        protected Vector2 nameFontSize = Vector2.Zero;
        private string syncName;


        public Human(EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont, bool damageable, Color nameColor) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, damageable)
        {
            this.nameFont = nameFont;
            this.NameColor = nameColor;
            BaseSize = (animationsByType[(int)EntityAnimation.IdleDown][0].Texture.Bounds.Size.ToVector2() * scale).ToPoint();
        }

        protected override void UpdateDrawOffset()
        {
            base.UpdateDrawOffset();
            nameOffset = new Vector2(BaseSize.X / 2 - nameFontSize.X / 2, healthBarOffset.Y - 2 - nameFontSize.Y);
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            if (!isHidenCompletely)
            {
                if (!hasAuthority || !isDead)
                {
                    sprite.DrawString(nameFont, SyncName, Location + nameOffset, NameColor, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, Layer);
                }
            }
        }

        protected void SetName(string name)
        {
            SyncName = name;
        }

        public virtual void OnNameSet()
        {
            nameFontSize = nameFont.MeasureString(SyncName);
            UpdateDrawOffset();
        }

        public string GetName()
        {
            return SyncName;
        }
    }
}

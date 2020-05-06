using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;

namespace RPGMultiplayerGame.Ui
{
    public class HealthBar : UiTextureComponent
    {

        public class Health : UiTextureComponent
        {
            private readonly Func<float> healthTextFunc;
            private readonly float maxHealth;

            public Health(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer, Func<float> healthTextFunc, float maxHealth) : base(origin, originType, defaultVisibility, layer, UiManager.Instance.UiHealthBar)
            {
                this.healthTextFunc = healthTextFunc;
                this.maxHealth = maxHealth;
            }

            public override void Draw(SpriteBatch sprite)
            {
                UpdateHealthSize();
                base.Draw(sprite);
            }
            private void UpdateHealthSize()
            {
                RenderRigion = new Rectangle(RenderRigion.Location, new Point((int)(healthTextFunc.Invoke() * Size.X / maxHealth), (int)Size.Y));
            }

        }
        public override bool IsVisible
        {
            get => base.IsVisible; set
            {
                base.IsVisible = value;
                health.IsVisible = base.IsVisible;
                healthText.IsVisible = base.IsVisible;
            }
        }
        private readonly UiTextComponent healthText;
        private readonly UiTextureComponent health;

        public HealthBar(Func<Point, Vector2> origin, PositionType originType, Func<float> healthTextFunc, float maxHealth) : base(origin, originType, true, UiManager.GUI_LAYER, UiManager.Instance.UiHealthBarBackground)
        {
            health = new Health(origin, originType, true, UiManager.GUI_LAYER * 0.1f, healthTextFunc, maxHealth);
            healthText = new UiTextComponent((windowSize) => Position + Size / 2, PositionType.Centered, true, UiManager.GUI_LAYER * 0.01f, UiManager.Instance.HealthTextFont, () => healthTextFunc.Invoke().ToString(), Color.Crimson);
        }
    }
}

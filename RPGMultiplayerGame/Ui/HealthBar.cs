using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;

namespace RPGMultiplayerGame.Ui
{
    public class HealthBar : UiTextureComponent
    {
        public override bool IsVisible
        {
            get => base.IsVisible; set
            {
                base.IsVisible = value;
                health.IsVisible = isVisible;
                healthText.IsVisible = isVisible;
            }
        }
        private readonly UiTextComponent healthText;
        private readonly UiTextureComponent health;
        private readonly Func<float> healthFunc;
        private readonly float maxHealth;

        public HealthBar(Func<Point, Vector2> origin, PositionType originType, Func<float> healthTextFunc, float maxHealth) : base(origin, originType, true, UiManager.GUI_LAYER, UiManager.Instance.UiHealthBarBackground)
        {
            health = new UiTextureComponent(origin, originType, true, UiManager.GUI_LAYER * 0.1f, UiManager.Instance.UiHealthBar);
            healthText = new UiTextComponent((windowSize) => Position + Size / 2, PositionType.Centered, true, UiManager.GUI_LAYER * 0.01f, UiManager.Instance.HealthTextFont, () => healthTextFunc.Invoke().ToString(), Color.Crimson);
            this.healthFunc = healthTextFunc;
            this.maxHealth = maxHealth;
        }

        private void UpdateHealthSize()
        {
            health.RenderRigion = new Rectangle(health.RenderRigion.Location, new Point((int)(healthFunc.Invoke() * health.Size.X / maxHealth), (int)health.Size.Y));
        }

        public override void Draw(SpriteBatch sprite)
        {
            UpdateHealthSize();
            base.Draw(sprite);
        }
    }
}

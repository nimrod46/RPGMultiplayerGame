using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Ui
{
    public class HealthBar : UiTextureComponent
    {
        private readonly UiTextComponent healthText;
        private readonly UiTextureComponent health;
        private readonly Func<float> healthFunc;
        private readonly float maxHealth;

        public HealthBar(Func<Point, Vector2> origin, PositionType originType, Func<float> healthTextFunc, float maxHealth) : base(origin, originType, true, GameManager.GUI_LAYER, GameManager.Instance.UiHealthBarBackground)
        {
            health = new UiTextureComponent(origin, originType, true, GameManager.GUI_LAYER * 0.1f, GameManager.Instance.UiHealthBar);
            healthText = new UiTextComponent((windowSize) => Position + Size / 2, PositionType.Centered, true, GameManager.GUI_LAYER * 0.01f, GameManager.Instance.PlayerNameFont, () => healthTextFunc.Invoke().ToString());
            this.healthFunc = healthTextFunc;
            this.maxHealth = maxHealth;
        }

        private void UpdateHealthSize()
        {
            health.RenderRigion = new Rectangle(health.RenderRigion.Location,new Point((int) (healthFunc.Invoke() * health.Size.X / maxHealth), (int) health.Size.Y));
        }

        public override void Draw(SpriteBatch sprite)
        {
            UpdateHealthSize();
            base.Draw(sprite);
            health.Draw(sprite);
            healthText.Draw(sprite);
        }
    }
}

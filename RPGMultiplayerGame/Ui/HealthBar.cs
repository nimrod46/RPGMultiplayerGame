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
        public float Health
        {
            get => float.Parse(healthText.Text); set
            {
                healthText.Text = value.ToString();
                UpdateHealthSize();
            }
        }

        public float MaxHealth
        {
            get => maxHealth; set
            {
                maxHealth = value;
                UpdateHealthSize();
            }
        }

        private readonly UiTextComponent healthText;
        private readonly UiTextureComponent health;
        private float maxHealth;

        public HealthBar(Func<Point, Vector2> origin, PositionType originType, float defaultHealth, float maxHealth) : base(origin, originType, GameManager.GUI_LAYER, GameManager.Instance.UiHealthBarBackground)
        {
            health = new UiTextureComponent(origin, originType, GameManager.GUI_LAYER * 0.1f, GameManager.Instance.UiHealthBar);
            healthText = new UiTextComponent((windowSize) => Position + Size / 2, PositionType.Centered, GameManager.GUI_LAYER * 0.01f, GameManager.Instance.PlayerNameFont, defaultHealth.ToString());
            this.MaxHealth = maxHealth;
        }

        private void UpdateHealthSize()
        {
            health.RenderRigion = new Rectangle(health.RenderRigion.Location,new Point((int) (Health * health.Size.X / maxHealth), (int) health.Size.Y));
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            health.Draw(sprite);
            healthText.Draw(sprite);
        }
    }
}

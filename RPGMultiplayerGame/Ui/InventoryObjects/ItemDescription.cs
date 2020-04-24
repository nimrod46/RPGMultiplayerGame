using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Ui.InventoryObjects
{
    public class ItemDescription : UiTextureComponent
    {
        public override bool IsVisible
        {
            get => base.IsVisible; set
            {
                base.IsVisible = value;
                descriptionText.IsVisible = IsVisible;
            }
        }

        private readonly UiTextComponent descriptionText;

        public ItemDescription(Func<Point, Vector2> origin, PositionType originType, float layer, Func<string> description) : base(origin, originType, false, layer, GameManager.Instance.ItemDescription)
        {
            this.descriptionText = new UiTextComponent((s) => new Vector2(8, 25), originType, false, layer * 0.1f, GameManager.Instance.PlayerNameFont, description);
            descriptionText.Parent = this;
        }

        public override void Draw(SpriteBatch sprite)
        {
            if (isVisible)
            {
                float xScale = 1;
                if (descriptionText.Size.X + descriptionText.Position.X * 2 + 5 > Size.X)
                {
                    xScale = (descriptionText.Size.X + descriptionText.Position.X * 2 + 5 - Size.X) / Size.X + 1;
                }

                float yScale = 1;
                if(descriptionText.Size.Y + descriptionText.Position.Y * 2 + 5 > Size.Y)
                {
                    yScale = (descriptionText.Size.Y + descriptionText.Position.Y * 2 + 5 - Size.Y) / Size.Y + 1;
                }
                Scale = MathHelper.Max(xScale, yScale);
                base.Draw(sprite);
            }
        }
    }
}

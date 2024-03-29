﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;

namespace RPGMultiplayerGame.Ui.InventoryObjects
{
    public class ItemDescription : UiTextureComponent
    {
        public override bool IsVisible
        {
            get => base.IsVisible; set
            {
                base.IsVisible = value;
                descriptionText.IsVisible = base.IsVisible;
                if (!IsVisible)
                {
                    Scale = 1;
                    descriptionText.UpdatePosition();
                }
            }
        }

        private readonly UiTextComponent descriptionText;

        public ItemDescription(Func<Point, Vector2> origin, PositionType originType, float layer, Func<string> description) : base(origin, originType, false, layer, UiManager.Instance.ItemDescriptionBackground)
        {
            this.descriptionText = new UiTextComponent((s) => new Vector2(5, 15), originType, false, layer * 0.1f, UiManager.Instance.ItemDescriptionFont, description, Color.DeepPink);
            descriptionText.Parent = this;
        }

        public override void Draw(SpriteBatch sprite)
        {
            if (IsVisible)
            {
                float xScale = 1;
                if (descriptionText.Size.X + descriptionText.Position.X * 2 > Size.X / Scale)
                {
                    xScale = (descriptionText.Size.X + descriptionText.Position.X * 2 - Size.X / Scale) / (Size.X / Scale) + 1;
                }

                float yScale = 1;
                if (descriptionText.Size.Y + descriptionText.Position.Y * 2 > Size.Y / Scale)
                {
                    yScale = (descriptionText.Size.Y + descriptionText.Position.Y * 2 - Size.Y / Scale) / (Size.Y / Scale) + 1;
                }
                Scale = MathHelper.Max(xScale, yScale);
                base.Draw(sprite);
            }
        }
    }
}

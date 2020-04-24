﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Ui
{
    public class UiTextureComponent : UiComponent
    {
        private Texture2D texture;

        public Texture2D Texture
        {
            get => texture; set
            {
                texture = value;
                UpdatePosition();
            }
        }

        public Rectangle RenderRigion { get; set; }

        public UiTextureComponent(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer, Texture2D texture) : base(origin, originType, defaultVisibility, layer)
        {
            Texture = texture;
        }

        protected override void UpdatePosition()
        {
            if (texture != null)
            {
                RenderRigion = texture.Bounds;
                size = texture.Bounds.Size.ToVector2();
                base.UpdatePosition();
            }
        }


        public override void Draw(SpriteBatch sprite)
        {
            if (IsVisible)
            {
                sprite.Draw(Texture, Position, RenderRigion, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, Layer);
            }
        }
    }
}

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
    public class UiTextureComponent : UiComponent
    {
        public Texture2D Texture { get; set; }

        public Rectangle RenderRigion { get; set; }

        public UiTextureComponent(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer, Texture2D texture) : base(origin, originType, defaultVisibility, layer)
        {
            Texture = texture;
            RenderRigion = texture.Bounds;
            Size = texture.Bounds.Size.ToVector2();
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

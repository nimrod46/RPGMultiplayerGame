using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Xml.Serialization;

namespace RPGMultiplayerGame.Ui
{
    public class UiTextureComponent : UiComponent
    {

        [XmlIgnore]
        public Texture2D Texture
        {
            get => texture; set
            {
                texture = value;
                RenderRigion = texture.Bounds;
                Size = texture.Bounds.Size.ToVector2();
                UpdatePosition();
            }
        }

        public Rectangle RenderRigion { get; set; }
        private Texture2D texture;


        public UiTextureComponent(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer, Texture2D texture) : base(origin, originType, defaultVisibility, layer)
        {
            Texture = texture;
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            if (IsVisible)
            {
                sprite.Draw(Texture, DrawPosition, RenderRigion, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, Layer);
            }
        }
    }
}

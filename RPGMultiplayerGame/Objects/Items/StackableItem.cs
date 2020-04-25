using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items
{
    public abstract class StackableGameItem : InteractiveItem
    {
        public int Count { get; protected set; }

        private readonly SpriteFont spriteFont;
        private Vector2 textSize;


        public StackableGameItem(ItemType itemType, string name, int count) : base(itemType, name)
        {
            this.Count = count;
            spriteFont = GameManager.Instance.PlayerNameFont;
            textSize = spriteFont.MeasureString(count.ToString());
        }

        public override void Draw(SpriteBatch sprite, Vector2 location, float layer)
        {
            base.Draw(sprite, location, layer);
            sprite.DrawString(spriteFont, Count + "", location + new Vector2(Texture.Width, Texture.Height) + new Vector2(-textSize.X + 10, - textSize.Y / 2), Color.Orange, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer * 0.1f);
        }

        public void Add(StackableGameItem stackableItemToAdd)
        {
            Count += stackableItemToAdd.Count;
            textSize = spriteFont.MeasureString(Count.ToString());
        }

        public void Use()
        {
            Count--;
            textSize = spriteFont.MeasureString(Count.ToString());
            if(Count == 0)
            {
                ItemType = ItemType.None;
            }
        }      
    }
}

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
    public class GameItem
    {
        public Texture2D Texture { get; set; }
        public ItemType ItemType
        {
            get => itemType; set
            {
                itemType = value;
                Texture = UiManager.Instance.GetItemTextureByType(value);
            }
        }

        public string Name { get; set; }

        private ItemType itemType;

        public GameItem()
        {
            ItemType = ItemType.None;
            Name = "";
        }

        public GameItem(ItemType itemType, string name)
        {
            ItemType = itemType;
            Name = name;
        }

        public virtual void Draw(SpriteBatch sprite, Vector2 location, float layer)
        {
            sprite.Draw(Texture, location, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layer);
        }

        public bool IsExists()
        {
            return ItemType != ItemType.None;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

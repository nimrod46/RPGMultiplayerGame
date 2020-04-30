using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using System.Xml.Serialization;

namespace RPGMultiplayerGame.Objects.Items
{
    [XmlInclude(typeof(CommonSword))]
    [XmlInclude(typeof(CommonWond))]
    [XmlInclude(typeof(CommonHealthPotion))]
    [XmlInclude(typeof(BatClaw))]
    public class GameItem : NetworkIdentity
    {
        [XmlIgnore]
        private Texture2D texture;

        public ItemType SyncItemType
        {
            get => itemType; set
            {
                itemType = value;
                texture = UiManager.Instance.GetItemTextureByType(value);
            }
        }

        public string SyncName { get; set; }

        public Vector2 Size { get; private set; }
        protected float Scale
        {
            get => scale; set
            {
                scale = value;
                Size = texture.Bounds.Size.ToVector2() * scale;
            }
        }

        private float scale;
        private ItemType itemType;

        public GameItem()
        {
            SyncItemType = ItemType.None;
            SyncName = "";
            Scale = 1;
            Size = texture.Bounds.Size.ToVector2();
        }

        public GameItem(ItemType itemType, string name)
        {
            SyncItemType = itemType;
            SyncName = name;
            Scale = 1;
            Size = texture.Bounds.Size.ToVector2();
        }

        public virtual void Draw(SpriteBatch sprite, Vector2 location, float layer)
        {
            sprite.Draw(texture, location, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, layer);
        }

        public bool IsExists()
        {
            return SyncItemType != ItemType.None;
        }

        public override string ToString()
        {
            return SyncName;
        }
    }
}

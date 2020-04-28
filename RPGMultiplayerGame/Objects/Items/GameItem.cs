using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
namespace RPGMultiplayerGame.Objects.Items
{
    public class GameItem : NetworkIdentity
    {
        public Texture2D Texture { get; set; }
        public ItemType SyncItemType
        {
            get => itemType; set
            {
                itemType = value;
                Texture = UiManager.Instance.GetItemTextureByType(value);
            }
        }

        public string SyncName { get; set; }

        private ItemType itemType;

        public GameItem()
        {
            SyncItemType = ItemType.None;
            SyncName = "";
        }

        public GameItem(ItemType itemType, string name)
        {
            SyncItemType = itemType;
            SyncName = name;
        }

        public virtual void Draw(SpriteBatch sprite, Vector2 location, float layer)
        {
            sprite.Draw(Texture, location, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layer);
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

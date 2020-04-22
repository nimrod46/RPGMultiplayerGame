using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Ui;

namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public class ItemSlotUi<T> : UiTextureComponent where T : GameItem 
    {
        public T Item { get; set; }

        public ItemSlotUi(Func<Point, Vector2> origin, PositionType positionType) : base(origin, positionType, GameManager.GUI_LAYER, GameManager.Instance.InventorySlotBackground)
        {
        }

        public ItemSlotUi(Func<Point, Vector2> origin, PositionType positionType, T item) : base(origin, positionType, GameManager.GUI_LAYER, GameManager.Instance.InventorySlotBackground)
        {
            Item = item;
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            if (Item.IsExists())
            {
                Item.Draw(sprite, Position + new Vector2(Texture.Width / 2 - Item.Texture.Width / 2,
                    Texture.Height / 2 - Item.Texture.Height / 2)
                    ,GameManager.GUI_LAYER * 0.1f);
            }
        }
    }
}

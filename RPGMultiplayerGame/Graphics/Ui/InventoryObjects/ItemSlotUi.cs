using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Ui;
using RPGMultiplayerGame.Ui.InventoryObjects;
using System;
using RPGMultiplayerGame.Graphics.Ui;

namespace RPGMultiplayerGame.Objects.InventoryObjects
{
    public class ItemSlotUi<T> : UiTextureComponent where T : GameItem
    {
        public override bool IsVisible
        {
            get => base.IsVisible; set
            {
                base.IsVisible = value;
                if (!IsVisible)
                {
                    HideDescription();
                }
            }
        }

        public T Item
        {
            get => item; set
            {
                item = value;
                Item.SetAsUiItem(this, (g) => new Vector2(Texture.Width / 2, Texture.Height / 2), PositionType.Centered);
            }
        }
        protected ItemDescription description;
        private T item;

        public ItemSlotUi(Func<Point, Vector2> origin, PositionType positionType, bool defaultVisibility, T item) : base(origin, positionType, defaultVisibility, UiManager.GUI_LAYER, UiManager.Instance.InventorySlotBackground)
        {
            Item = item;
            description = new ItemDescription((s) => InputManager.Instance.MouseBounds().Location.ToVector2(), PositionType.TopLeft, UiManager.GUI_LAYER * 0.001f, () => Item.ToString());
        }

        public void ShowDescription()
        {
            description.IsVisible = true;
            GameManager.Instance.HideMouse = true;
        }

        public void HideDescription()
        {
            description.IsVisible = false;
            GameManager.Instance.HideMouse = false;
        }
    }
}

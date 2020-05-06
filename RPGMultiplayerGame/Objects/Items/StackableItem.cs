using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Ui;
using System;

namespace RPGMultiplayerGame.Objects.Items
{
    public abstract class StackableGameItem : InteractiveItem
    {
        public int SyncCount
        {
            get => syncCount; set
            {
                syncCount = value;
                InvokeSyncVarNetworkly(nameof(SyncCount), syncCount);
            }
        }

        private readonly SpriteFont spriteFont;
        private Vector2 textSize;
        private int syncCount;
        private UiTextComponent countText;

        public StackableGameItem(ItemType itemType, string name, int count) : base(itemType, name)
        {
            this.SyncCount = count;
            spriteFont = UiManager.Instance.StackableItemNumberFont;
            textSize = spriteFont.MeasureString(count.ToString());
            countText = new UiTextComponent((g) => new Vector2(Size.X, Size.Y), UiComponent.PositionType.ButtomRight, false, UiManager.GUI_LAYER * 0.01f, spriteFont, () => SyncCount.ToString(), Color.Orange);
        }

        public void Add(StackableGameItem stackableItemToAdd)
        {
            SyncCount += stackableItemToAdd.SyncCount;
            textSize = spriteFont.MeasureString(SyncCount.ToString());
            stackableItemToAdd.InvokeBroadcastMethodNetworkly(nameof(stackableItemToAdd.Destroy));
        }

        public override void SetAsMapItem(Vector2 location)
        {
            base.SetAsMapItem(location);
            countText.IsVisible = false;
        }

        public override void SetAsUiItem(UiComponent uiParent, Func<Point, Vector2> origin, UiComponent.PositionType originType)
        {
            base.SetAsUiItem(uiParent, origin, originType);
        }

        public void Use()
        {
            SyncCount--;
            textSize = spriteFont.MeasureString(SyncCount.ToString());
            if (SyncCount == 0)
            {
                SyncItemType = ItemType.None;
            }
        }
    }
}

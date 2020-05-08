using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.InventoryObjects;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Ui;
using System;
using System.Xml.Serialization;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Objects.Items
{
    [XmlInclude(typeof(CommonSword))]
    [XmlInclude(typeof(CommonWond))]
    [XmlInclude(typeof(CommonHealthPotion))]
    [XmlInclude(typeof(BatClaw))]
    [XmlInclude(typeof(CommonBow))]
    [XmlInclude(typeof(IceBow))]
    [XmlInclude(typeof(ExplodingBow))]
    [XmlInclude(typeof(StormBow))]
    public class GameItem : GraphicObject
    {
        public ItemType SyncItemType
        {
            get => itemType; set
            {
                itemType = value;
                Texture = UiManager.Instance.GetItemTextureByType(value);
                InvokeSyncVarNetworkly(nameof(SyncItemType), itemType);
            }
        }

        public string SyncName { get; set; }

        [XmlIgnore]
        public float UiScale { get { return uiItem.Scale; } set { uiItem.Scale = value; } }

        protected UiTextureComponent uiItem;
        private ItemType itemType;
        private UiComponent parent;
        public GameItem()
        {
            SyncItemType = ItemType.None;
            SyncName = "";
            Scale = 1;
            Layer = GraphicManager.ITEM_LAYER;
            uiItem = new UiTextureComponent((g) => Vector2.Zero, PositionType.Centered, false, UiManager.GUI_LAYER * 0.1f, Texture);
        }

        public GameItem(ItemType itemType, string name)
        {
            SyncItemType = itemType;
            SyncName = name;
            Scale = 1;
            Layer = GraphicManager.ITEM_LAYER;
            uiItem = new UiTextureComponent((g) => Vector2.Zero, PositionType.Centered, false, UiManager.GUI_LAYER * 0.1f, Texture);
        }

        public virtual void SetAsUiItem(UiComponent uiParent, Func<Point, Vector2> origin, PositionType originType)
        {
            InvokeBroadcastMethodNetworkly(nameof(SetAsUiItemLocaly));
            uiItem.Parent = uiParent;
            SyncIsVisible = false;
            uiItem.OriginFunc = origin;
            uiItem.OriginType = originType;
        }

        public void SetAsUiItemLocaly()
        {
            Size = Vector2.Zero.ToPoint();
        }

        public virtual void SetAsMapItem(Vector2 location)
        {
            InvokeBroadcastMethodNetworkly(nameof(SetAsMapItemLocaly));
            SyncX = location.X;
            SyncY = location.Y;
            SyncIsVisible = true;
            uiItem.Parent = null;
        }

        public void SetAsMapItemLocaly()
        {
            Size = (Texture.Bounds.Size.ToVector2() * Scale).ToPoint();
            uiItem.IsVisible = false;
        }

        public bool IsExists()
        {
            return SyncItemType != ItemType.None;
        }

        public void Delete()
        {
            SyncItemType = ItemType.None;
            OnDestroyed(this);
            uiItem.Delete();
        }

        public override string ToString()
        {
            return SyncName;
        }
    }
}

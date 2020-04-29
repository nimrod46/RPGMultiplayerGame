using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Dialogs;
using RPGMultiplayerGame.Objects.InventoryObjects;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class Blacksmith : MultipleInteractionNpc
    {
        private Inventory<GameItemShop> shop;

        public Blacksmith() : base(GraphicManager.EntityId.Blacksmith, 0, 0, 100, GraphicManager.Instance.PlayerNameFont)
        {
            SyncName = "Blacksmith";
            minDistanceForObjectInteraction = 40;
            scale = 0.3f;
        }
        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            shop = new Inventory<GameItemShop>((windowSize) => (windowSize.ToVector2() / 2), PositionType.Centered, false, 5, 5);
            shop.OnItemClickedEvent += Shop_OnItemClickedEvent;
            dialog = new ComplexDialog(SyncName, "Are you here to buy or what?", false);
            AddItemToShop(new GameItemShop(new CommonWond(), 50));
            AddItemToShop(new GameItemShop(new CommonHealthPotion() { SyncCount = 10 }, 10));
            AddItemToShop(new GameItemShop(new CommonSword(), 50));
        }

        protected override void CmdInteractWithPlayer(Player player, int dialogIndex)
        {
            shop.IsVisible = true;
            shop.IsIntractable = true;
            base.CmdInteractWithPlayer(player, dialogIndex);
        }

        public override void CmdStopInteractWithPlayer(Player player)
        {
            if (shop.IsVisible)
            {
                shop.IsVisible = false;
                shop.IsIntractable = false;
            }
            base.CmdStopInteractWithPlayer(player);

        }

        public void AddItemToShop(GameItemShop gameItem)
        {
            shop.TryAddItem(gameItem);
        }

        private void Shop_OnItemClickedEvent(GameItemShop item)
        {
            InvokeCommandMethodNetworkly(nameof(CmdCheckPlayerBuy), GameManager.Instance.Player, shop.GetItemSlot(item));
        }

        public void CmdCheckPlayerBuy(Player player, int slot)
        {
            if (shop.TryGetItemInSlot(slot, out GameItemShop itemShop))
            {
                if (player.IsAbleToBuy(itemShop))
                {
                    ServerManager.Instance.GivePlayerGameItem(player, itemShop.GameItem);
                    player.SyncGold -= itemShop.Price;
                }
            }
        }
    }
}

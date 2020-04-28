using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items
{
    public class GameItemShop : GameItem
    {
        public long Price { get; set; }

        public GameItem GameItem { get; }

        public GameItemShop() : base()
        {
            GameItem = new EmptyItem();
        }

        public GameItemShop(GameItem gameItem, long price) : base(gameItem.SyncItemType, gameItem.SyncName)
        {
            this.GameItem = gameItem;
            Price = price;
        }

        public override string ToString()
        {
            return GameItem.ToString() + "\n"
                + "Price: " + Price + " Gold";
        }
    }
}

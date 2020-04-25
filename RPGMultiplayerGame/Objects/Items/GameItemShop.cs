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
        }

        public GameItemShop(GameItem gameItem, long price) : base(gameItem.ItemType, gameItem.Name)
        {
            this.GameItem = gameItem;
            Price = price;
        }
    }
}

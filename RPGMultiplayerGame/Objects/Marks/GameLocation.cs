using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Marks
{
    public class GameLocation : GameObject
    {
        public static Vector2 DefaultGameLocation;

        public GameLocation()
        {
            DefaultGameLocation = Location;
        }

        public void MoveTo(Vector2 newLocation)
        {
            SyncX = newLocation.X;
            SyncY = newLocation.Y;
        }
    }
}

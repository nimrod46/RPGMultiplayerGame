using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Other
{
    public class GameMap
    {
        private readonly List<MapObject> mapObjects = new List<MapObject>();

        public void AddBlock(MapObject mapObject)
        {
            mapObjects.Add(mapObject);
        }

        public void RemoveBlock(MapObject mapObject)
        {
            mapObjects.Add(mapObject);
        }

        public bool TryGetHighBlockAt<T>(Rectangle rectangle, out T outBlock) where T : MapObject
        {
            return TryGetBlockAt(rectangle, 1, out outBlock);
        }

        public bool TryGetBlockAt<T>(Rectangle rectangle, int layer, out T outBlock) where T : MapObject
        {
            outBlock = null;
            foreach (var mapObject in mapObjects)
            {
                if (mapObject is T type && mapObject.SyncLayer >= layer && mapObject.GetBoundingRectangle().Intersects(rectangle))
                {
                    outBlock = type;
                    return true;
                }
            }
            return false;
        }
    }
}

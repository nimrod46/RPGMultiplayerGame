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
            lock (mapObjects)
            {
                mapObjects.Add(mapObject);
            }
        }

        public void RemoveBlock(MapObject mapObject)
        {
            lock (mapObjects)
            {
                mapObjects.Remove(mapObject);
            }
        }

        public bool TryGetHighBlockAt<T>(Rectangle rectangle, out T outBlock) where T : MapObject
        {
            return TryGetBlockAt(rectangle, 1, true, out outBlock);
        }

        public bool TryGetBlockAt<T>(Rectangle rectangle, int layer, bool onlyBlocking, out T outBlock) where T : MapObject
        {
            lock (mapObjects)
            {
                outBlock = null;
                foreach (var mapObject in mapObjects)
                {
                    if (mapObject is T block && mapObject.SyncLayer >= layer && mapObject.GetBoundingRectangle().Intersects(rectangle))
                    {
                        if (!onlyBlocking || !(mapObject is SpecialBlock specialBlock) || specialBlock.Isblocking())
                        {
                            outBlock = block;
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}

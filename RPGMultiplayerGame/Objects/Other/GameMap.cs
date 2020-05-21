using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Extention;
using RPGMultiplayerGame.Objects.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;

namespace RPGMultiplayerGame.Objects.Other
{
    public class GameMap
    {
        private readonly MapObject[,] mapObjects = new MapObject[10000,10000];

        public void AddBlock(MapObject mapObject)
        {
            if(mapObjects[(int)mapObject.SyncX / 16, (int)mapObject.SyncY / 16] != null)
            {
                if(mapObjects[(int)mapObject.SyncX / 16, (int)mapObject.SyncY / 16].SyncLayer > mapObject.SyncLayer)
                {
                    return;
                }
            }
            mapObjects[(int)mapObject.SyncX / 16, (int)mapObject.SyncY / 16] = mapObject;
        }

        public void RemoveBlock(MapObject mapObject)
        {
            mapObjects[(int)mapObject.SyncX / 16, (int)mapObject.SyncY / 16] = null;
        }

        public bool TryGetHighBlockAt<T>(Rectangle rectangle, out T outBlock) where T : MapObject
        {
            return TryGetBlockAt(rectangle, true, out outBlock);
        }

        public bool TryGetBlockAt<T>(Rectangle rectangle, bool onlyBlocking, out T outMapObject) where T : MapObject
        {
            //lock (mapObjects)
            //{
            //    outBlock = null;
            //    foreach (var mapObject in mapObjects)
            //    {
            //        if (mapObject is T block && mapObject.SyncLayer >= layer && mapObject.GetBoundingRectangle().Intersects(rectangle))
            //        {
            //            if (!onlyBlocking || !(mapObject is SpecialBlock specialBlock) || specialBlock.Isblocking())
            //            {
            //                outBlock = block;
            //                return true;
            //            }
            //        }
            //    }
            //    return false;
            //}
            outMapObject = null;
            for (int i = rectangle.X / 16; i <= rectangle.X / 16 + rectangle.Width / 16 + 1; i++)
            {
                for (int j = rectangle.Y / 16; j <= rectangle.Y / 16 + rectangle.Height / 16 + 1; j++)
                {
                    if (i < 0 || i >= mapObjects.GetLength(0) || j < 0 ||  j >= mapObjects.GetLength(1))
                    {
                        continue;
                    }
                    if (mapObjects[i, j] is T mapObj)
                    {
                        if (mapObj.GetBoundingRectangle().Intersects(rectangle))
                        {
                            if (!onlyBlocking || mapObj.Isblocking())
                            {
                                outMapObject = mapObj;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool GetPathTo(Vector2 source, Vector2 destination, out List<Vector2> waypoints)
        {
            waypoints = new List<Vector2>();
            source = new Vector2(Round((int) source.X, 16), Round((int) source.Y, 16));
            destination = new Vector2(Round((int)destination.X, 16), Round((int)destination.Y, 16));
            alreadyChecked.Clear();
            Vector2 refSource = source;
            if (GetPathTo(ref refSource, destination, Operations.GetDirection(source - destination), ref waypoints))
            {
                Console.WriteLine("----------------");
                waypoints.Insert(0, source);
                foreach (var item in waypoints)
                {
                    Console.WriteLine("FOUNDDDDD: " + item);
                    Console.WriteLine(GetMapObjectAt(item).Isblocking());
                    Console.WriteLine((int)item.X / 16);
                    Console.WriteLine((int)item.Y / 16);
                }
                return true;
            }
            else
            {
                Console.WriteLine("No path found");
            }
            return false;
        }

        readonly List<Vector2> alreadyChecked = new List<Vector2>();

        public bool GetPathTo(ref Vector2 source, Vector2 destination, Direction direction, ref List<Vector2> waypoints)
        {
            if (GetMapObjectAt(source) == null || GetMapObjectAt(source).Isblocking())
            {
                Console.WriteLine("NOPE");
                return false;
            }

            if (source == destination)
            {
                waypoints.Add(source);
                return true;
            }
            //if(GetMapObjectAt(source) != null)

            if (direction == Direction.Left || direction == Direction.Right)
            {
                if (source.X == destination.X)
                {
                    return GetPathTo(ref source, destination, Operations.GetDirection(source - destination), ref waypoints);
                }
            }
            else
            {
                if (source.Y == destination.Y)
                {
                    return GetPathTo(ref source, destination, Operations.GetDirection(source - destination), ref waypoints);
                }
            }
            Vector2 vector2 = MoveVectorByDirection(source, 16, direction);
            if (waypoints.Contains(vector2))
            {
                return false;
            }

            waypoints.Add(vector2);
            if (GetMapObjectAt(vector2) == null || GetMapObjectAt(vector2).Isblocking())
            {
                if (alreadyChecked.Contains(vector2))
                {
                    return false;
                }
                else
                {
                    alreadyChecked.Add(vector2);
                }
                waypoints.Remove(vector2);
                vector2 = MoveVectorByDirection(vector2, -16, direction);
                List<Vector2> tempWaypoints = new List<Vector2>(waypoints);
                List<Vector2> tempWaypoints1 = new List<Vector2>(waypoints);
                List<Vector2> tempWaypoints2 = new List<Vector2>(waypoints);
                if (!GetPathTo(ref vector2, destination, GetDirectionByIndex((int)direction + 1), ref tempWaypoints))
                {
                    // return true;
                    tempWaypoints.Clear();
                }
                 if (!GetPathTo(ref vector2, destination, GetDirectionByIndex((int)direction + 2), ref tempWaypoints1))
                {
                    // return true;
                    tempWaypoints1.Clear();
                }
                 if (!GetPathTo(ref vector2, destination, GetDirectionByIndex((int)direction + 3), ref tempWaypoints2))
                {
                    //  return true;
                    tempWaypoints2.Clear();
                }
                if(!tempWaypoints.Any() && !tempWaypoints1.Any() && !tempWaypoints2.Any())
                {
                    return false;
                }
                else
                {
                    if (tempWaypoints.Any() && tempWaypoints1.Any())
                    {
                        tempWaypoints = tempWaypoints.Count < tempWaypoints1.Count ? tempWaypoints : tempWaypoints1;
                    }
                    else if(tempWaypoints1.Any())
                    {
                        tempWaypoints = tempWaypoints1;
                    }

                    if (tempWaypoints.Any() && tempWaypoints2.Any())
                    {
                        tempWaypoints = tempWaypoints.Count < tempWaypoints2.Count ? tempWaypoints : tempWaypoints2;
                    }
                    else if (tempWaypoints2.Any())
                    {
                        tempWaypoints = tempWaypoints2;
                    }


                    waypoints = tempWaypoints;
                    return true;
                }
            }
            else
            {
                return GetPathTo(ref vector2, destination, direction, ref waypoints);
            }
        }

        private Direction GetDirectionByIndex(int index) 
        {
            if (index < 0) 
            {
                return (Direction) (Enum.GetValues(typeof(Direction)).Length + index);
            }
            if(index > Enum.GetValues(typeof(Direction)).Length - 1)
            {
                return (Direction)(index - Enum.GetValues(typeof(Direction)).Length);
            }
            return (Direction)index;
        }

        private Vector2 MoveVectorByDirection(Vector2 source, int amount, Direction direction)
        {
            return direction switch
            {
                Direction.Left => source + new Vector2(-amount, 0),
                Direction.Up => source + new Vector2(0, -amount),
                Direction.Right => source + new Vector2(amount, 0),
                Direction.Down => source + new Vector2(0, amount),
                _ => Vector2.Zero,
            };
        }

        public MapObject GetMapObjectAt(Vector2 location)
        {
            int i = (int)location.X / 16;
            int j = (int)location.Y / 16;
            if (i < 0 || i >= mapObjects.GetLength(0) || j < 0 || j >= mapObjects.GetLength(1))
            {
                return null;
            }
            return mapObjects[i, j];
        }

        private int Round(int a, int b)
        {
            if (a % b == 0)
            {
                return a;
            }
            return a - (a % b);

        }
    }
}

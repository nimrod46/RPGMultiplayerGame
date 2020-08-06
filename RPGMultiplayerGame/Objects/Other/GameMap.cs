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

        public bool HasClearSight(Vector2 source, Vector2 destination)
        {
            source = new Vector2(Round((int)source.X, 16), Round((int)source.Y, 16));
            destination = new Vector2(Round((int)destination.X, 16), Round((int)destination.Y, 16));
            return HasClearSight(source, destination, Operations.GetDirection(source - destination));
        }

        public bool HasClearSight(Vector2 source, Vector2 destination, Direction direction)
        {
            source = MoveVectorByDirection(source, 16, direction);
            if (GetMapObjectAt(source) == null || GetMapObjectAt(source).Isblocking())
            {
                Console.WriteLine("No clear sight");
                return false;
            }

            if (source == destination)
            {
                Console.WriteLine("CLEAR SIGHT!");
                return true;
            }
            return HasClearSight(source, destination, Operations.GetDirection(source - destination));
        }

        public bool GetPathTo(Vector2 source, Vector2 destination, out List<Vector2> waypoints)
        {
            waypoints = new List<Vector2>();
            source = new Vector2(Round((int)source.X, 16), Round((int)source.Y, 16));
            destination = new Vector2(Round((int)destination.X, 16), Round((int)destination.Y, 16));
            alreadyChecked.Clear();
            Vector2 refSource = source;
            //Console.WriteLine("start: " + source);
            //Console.WriteLine("start: " + source / 16);
            if (GetPathTo(refSource, destination, Operations.GetDirection(source - destination), ref waypoints, false))
            {
                //Console.WriteLine("----------------");
                waypoints.Insert(0, source);
                foreach (var item in waypoints)
                {
                    //Console.WriteLine("FOUNDDDDD: " + item);
                    //Console.WriteLine(GetMapObjectAt(item).Isblocking());
                    //Console.WriteLine((int)item.X / 16);
                    //Console.WriteLine((int)item.Y / 16);
                }
                return true;
            }
            else
            {
                //Console.WriteLine("No path found");
            }
            return false;
        }

        readonly List<Vector2> alreadyChecked = new List<Vector2>();

        public bool GetPathTo(Vector2 source, Vector2 destination, Direction direction, ref List<Vector2> waypoints, bool shouldCheck)
        {
            if (shouldCheck)
            {
                source = MoveVectorByDirection(source, 16, direction);
                if (alreadyChecked.Contains(source))
                {
                    return false;
                }
                alreadyChecked.Add(source);
                if (GetMapObjectAt(source) == null || GetMapObjectAt(source).Isblocking())
                {
                    return false;
                }
                waypoints.Add(source);
            }

            if (source == destination)
            {
                return true;
            }

            Vector2 heading = source - destination;
            
            Direction nextBestDirection = Operations.GetDirection(heading);
            List<Vector2> tempWaypoints = new List<Vector2>(waypoints);
            //Console.WriteLine("trying best: " + nextBestDirection);
            if (GetPathTo(source, destination, nextBestDirection, ref tempWaypoints, true))
            {
                waypoints = tempWaypoints;
                //Console.WriteLine("best: " + nextBestDirection + " correct");
                return true;
            }

            Direction nextGoodDirection;
            if (nextBestDirection == Direction.Left || nextBestDirection == Direction.Right)
            {
                nextGoodDirection = Operations.GetDirectionOnYAxis(heading);
            }
            else
            {
                nextGoodDirection = Operations.GetDirectionOnXAxis(heading);
            }
            //Console.WriteLine("trying good: " + nextGoodDirection);
            tempWaypoints = new List<Vector2>(waypoints);
            if (GetPathTo(source, destination, nextGoodDirection, ref tempWaypoints, true))
            {
                //Console.WriteLine("good: " + nextGoodDirection + " correct");
                waypoints = tempWaypoints;
                return true;
            }
            tempWaypoints = new List<Vector2>(waypoints);
            List<Vector2> resultWaypoints = new List<Vector2>();
           // Console.WriteLine("trying best opposite: " + GetDirectionByIndex((int)nextBestDirection + 2));
            if (GetPathTo(source, destination, GetDirectionByIndex((int)nextBestDirection + 2), ref tempWaypoints, true))
            {
                resultWaypoints = tempWaypoints;
            }

            List<Vector2> tempWaypoints2 = new List<Vector2>(waypoints);
            //Console.WriteLine("trying good opposite: " + GetDirectionByIndex((int)nextGoodDirection + 2));
            if (GetPathTo(source, destination, GetDirectionByIndex((int)nextGoodDirection + 2), ref tempWaypoints2, true))
            {
                if(resultWaypoints.Count  == 0 || resultWaypoints.Count > tempWaypoints2.Count)
                {
                    resultWaypoints = tempWaypoints2;
                }
            }

            if (resultWaypoints.Count == 0) 
            {
                return false;
            }
            waypoints = resultWaypoints;
            return true;
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

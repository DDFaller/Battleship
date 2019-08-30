using System.Collections.Generic;

using Tecgraf.Battleship.Core;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;
using UnityEngine;

namespace Tecgraf.Battleship.Strategies
{
    public class LeoSeparate : IPlacementStrategy
    {
        public int OverallRetryCount { get; set; } = -1;
        public int IndividualRetryCount { get; set; } = 20;
        public struct Point
        {
            public int x;
            public int y;

        }

        public List<Point> points = new List<Point>();

        public Point NewPoint(int x, int y)
        {
            Point p;
            p.x = x;
            p.y = y;
            return p;
        }

        public void InitPontos()
        {
            for(int i = 0; i<10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    points.Add(NewPoint(i, j));
                }
            }
        }

        public bool CheckForPoint(int x, int y)
        {
            for(int i = 0; i < points.Count; i++)
            {
                if(x==points[i].x && y == points[i].y)
                {
                    return true;
                }
            }
            return false;
        }

        private void RemovePointSpace(int x, int y)
        {
            Point p;
            
            for (int i=0; i<points.Count; i++)
            {
                try
                {
                    p.x = x; p.y = y;
                    if (points.Contains(p))
                    {
                        points.Remove(p);
                    }

                    p.x = x+1; p.y = y;
                    if (points.Contains(p))
                    {
                        points.Remove(p);
                    }
                    p.x = x-1; p.y = y;
                    if (points.Contains(p))
                    {
                        points.Remove(p);
                    }
                    p.x = x; p.y = y+1;
                    if (points.Contains(p))
                    {
                        points.Remove(p);
                    }
                    p.x = x; p.y = y-1;
                    if (points.Contains(p))
                    {
                        points.Remove(p);
                    }
                }
                catch (System.Exception e) {
                    Debug.Log("points.Count = " + points.Count);
                    Debug.Log("point:(" + x + ',' + y + ')');
                    Debug.Break();
                }
            }
        }

        private void RemoveShipSpace(ShipPlacementOrientations dir, IShip ship, int x, int y)
        {
            if (dir == ShipPlacementOrientations.Horizontal)
            {
                for (int i = 0; i < ship.Size; i++)
                {
                    RemovePointSpace(x+i,y);
                }
            }
            else
            {
                for (int i = 0; i < ship.Size; i++)
                {
                    RemovePointSpace(x,y+i);
                }
            }
        }

        
        
        public bool SpaceAvaiable(ShipPlacementOrientations dir, IShip ship, int x, int y)
        {
            Point p;
            if (dir == ShipPlacementOrientations.Horizontal)
            {
                for (int i = 0; i < ship.Size; i++)
                {
                    p.x = x + i;
                    p.y = y;
                    if (!points.Contains(p)) return false;
                }
            }
            else
            {
                for (int i = 0; i < ship.Size; i++)
                {
                    p.x = x;
                    p.y = y + i;
                    if (!points.Contains(p)) return false;
                }
            }

            return true;
        }


        public void PlaceShips(Board board, List<IShip> ships)
        {
            bool success = false;
            int attempts = 0;

            while (!success && (attempts < OverallRetryCount || OverallRetryCount < 0))
            {
                points.Clear();
                InitPontos();
                success = true;
                attempts++;
               

                foreach (var ship in ships)
                {


                    var dir = (ShipPlacementOrientations)(Random.Range((int)0, (int)2));
                    int maxCoordX = board.GridSize - (dir == ShipPlacementOrientations.Horizontal ? ship.Size : 0);
                    int maxCoordY = board.GridSize - (dir == ShipPlacementOrientations.Vertical ? ship.Size : 0);

                    for (int i = 0; IndividualRetryCount < 0 || i < IndividualRetryCount; i++)
                    {
                        int x;
                        int y;
                        int aux = 0;

                        do
                        {
                            aux++;
                            x = Random.Range(0, maxCoordX);
                            y = Random.Range(0, maxCoordY);
                            
                        } while (!points.Contains(NewPoint(x, y)) && aux<30);

                        if (SpaceAvaiable(dir, ship, x, y) && board.PlaceShip(ship, dir, x, y))
                        {
                            RemoveShipSpace(dir, ship, x, y);
                            break;
                        }
                        
                        if (i == IndividualRetryCount - 1)
                        {
                            board.Clear();
                            success = false;
                        }
                    }

                    if (!success)
                        break;
                }
            }
        }
    }
}

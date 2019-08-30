using System.Collections.Generic;

using Tecgraf.Battleship.Core;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;
using UnityEngine;

namespace Tecgraf.Battleship.Strategies
{
    public class SeparatePlacementStrategy : IPlacementStrategy
    {
        public int OverallRetryCount { get; set; } = -1;
        public int IndividualRetryCount { get; set; } = 50;
        private struct Point
        {
            public int X;
            public int Y;
        }
        private List<Point> AvailablePositions = new List<Point>();
        private List<IShip> allShipsAlreadyPlaceds = new List<IShip>();

        

        public void PlaceShips(Board board, List<IShip> ships)
        {
            ResetPositions(AvailablePositions);
            
            
            bool success = false;
            int attempts = 0;
          

            do
            {
                   
                for (int i = ships.Count -1 ; i >= 0; i--)
                {
                    Point p = AvailablePositions[Random.Range(0, AvailablePositions.Count)]; ;
                    var ship = ships[i];
                    
                    ship.Orientation = (ShipPlacementOrientations)Random.Range(0, 2);
                    if (false)
                    {
                        i++;
                    }
                    else
                    {
                        int coordX = p.X;
                        int coordY = p.Y;
                        if (ShipCanFit(AvailablePositions, new Point() { X = coordX, Y = coordY }, ship))
                        {
                            board.PlaceShip(ship, ship.Orientation, coordX, coordY);
                            RemoveShipPosition(AvailablePositions, new Point() { X = coordX, Y = coordY }, ship);

                            allShipsAlreadyPlaceds.Add(ship);
                            if (allShipsAlreadyPlaceds.Count == ships.Count)
                            {
                                success = true;
                                break;
                            }
                        }
                        else i++;
                        attempts++;
                        if (attempts > 500)
                        {
                            ResetTry(board, AvailablePositions);
                            i = ships.Count -1;
                            attempts = 0;
                            break;
                        }
                    }
                }
            } while (!success);
                   
            
            
        }
        private void ResetTry(Board board, List<Point> list)
        {
            board.Clear();
            ResetPositions(list);
            allShipsAlreadyPlaceds.Clear();
        }
        private void ResetPositions(List<Point> list)
        {
            list.Clear();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    list.Add(new Point() { X = i, Y = j });
                }
            }

        }
        private void RemoveShipPosition(List<Point> list, Point p, IShip ship)
        {
            for (int i = 0; i < ship.Size; i++)
            {
                //Debug.LogFormat("I : {0} X: {1} Y: {2} Orientation: {3} Size: {4}", i, p.X, p.Y,ship.Orientation,ship.Size);
                if (ship.Orientation == ShipPlacementOrientations.Horizontal)
                {

                    list.Remove(new Point() { X = p.X + i, Y = p.Y });
                    //Casulo
                    Point lower = new Point() { X = p.X + i, Y = p.Y - 1 };
                    if (list.Contains(lower)) list.Remove(lower);
                    Point upper = new Point() { X = p.X + i, Y = p.Y + 1 };
                    if (list.Contains(upper)) list.Remove(upper);
                    //
                }
                else
                {
                    list.Remove(new Point() { X = p.X, Y = p.Y + i });
                    //Casulo
                    Point left = new Point() { X = p.X - 1, Y = p.Y + i };
                    if (list.Contains(left))list.Remove(left);
                    Point right = new Point() { X = p.X + 1, Y = p.Y + i };
                    if(list.Contains(right))list.Remove(right);
                    //
                }
            }

            if (ship.Orientation == ShipPlacementOrientations.Horizontal)
            {
                if (list.Contains(new Point() { X = p.X - 1, Y = p.Y })) list.Remove(new Point() { X = p.X - 1, Y = p.Y });
                if (list.Contains(new Point() { X = p.X + ship.Size, Y = p.Y })) list.Remove(new Point() { X = p.X + ship.Size, Y = p.Y });
            }
            else
            {
                if (list.Contains(new Point() { X = p.X, Y = p.Y - 1 })) list.Remove(new Point() { X = p.X, Y = p.Y - 1 });
                if (list.Contains(new Point() { X = p.X, Y = p.Y + ship.Size })) list.Remove(new Point() { X = p.X, Y = p.Y + ship.Size });
            }
                RemoveSoloPoints(list);
        }
        private void RemoveSoloPoints(List<Point> list)
        {
            List<Point> listClone = new List<Point>(list);
            foreach (Point p in listClone)
            {
                if ((!list.Contains(new Point() { X = p.X - 1, Y = p.Y }) && !list.Contains(new Point() { X = p.X + 1, Y = p.Y }) && !list.Contains(new Point() { X = p.X, Y = p.Y - 1 }) && !list.Contains(new Point() { X = p.X, Y = p.Y + 1 })))
                {
                    list.Remove(p);

                }
            }
        }
        private bool ShipCanFit(List<Point> list, Point p, IShip ship)
        {

            int maxCoordX = 10 - (ship.Orientation == ShipPlacementOrientations.Horizontal ? ship.Size : 0);
            int maxCoordY = 10 - (ship.Orientation == ShipPlacementOrientations.Vertical ? ship.Size : 0);

            if (p.X  > maxCoordX || p.X + ship.Size  > 9) return false;
            if (p.Y > maxCoordY || p.Y + ship.Size > 9) return false;
            for (int i = 0; i < ship.Size; i++)
            {
                if (ship.Orientation == ShipPlacementOrientations.Horizontal)
                {
                    if (!list.Contains(new Point() { X = p.X + i, Y = p.Y })) return false;
                }
                else
                {
                    if (!list.Contains(new Point() { X = p.X, Y = p.Y + i })) return false; 
                }
            }
            
            return true;
        }
        private bool CalculateShipDirection(List<Point> list,IShip ship,out Point p,Board board)
        {
            p = AvailablePositions[Random.Range(0, AvailablePositions.Count)];
            int right = AvailableSpaceForRight(list,p,board,ship.Size);
            int left = AvailableSpaceForLeft(list, p, board, ship.Size);
            int down = AvailableSpaceForDown(list, p, board, ship.Size);
            int up = AvailableSpaceForUp(list, p, board, ship.Size);
            if(right + left >= ship.Size)
            {
                ship.Orientation = ShipPlacementOrientations.Horizontal;
                p = new Point() { X = p.X - left, Y = p.Y };
                return true;
            }
            if (down + up  >= ship.Size)
            {
                ship.Orientation = ShipPlacementOrientations.Vertical;
                p = new Point() { X = p.X , Y = p.Y - up };
                return true;
            }
            return false;
            
        }
        private bool AvailableSpaceInRown(List<Point> list, Point p, Board knowBoard, int size)
        {
            if (AvailableSpaceForLeft( list, p, knowBoard, size) + AvailableSpaceForRight(list, p, knowBoard, size) + 1 < size) return false;
            return true;
        }
        private bool AvailableSpaceInColumn(List<Point> list, Point p, Board knowBoard, int size)
        {
            if (AvailableSpaceForUp(list, p, knowBoard, size) + AvailableSpaceForDown(list, p, knowBoard, size) + 1 < size) return false;
            return true;
        }
        #region AvailableSpace
        private int AvailableSpaceForLeft(List<Point> list,Point p, Board knowBoard, int size)
        {
            int left = 0;
            Point h = new Point() { X = p.X, Y = p.Y };
            for (int i = 0; i < size && h.X > 0; i++)
            {
                h.X--;
                if (!list.Contains(new Point() { X = h.X, Y = p.Y })) break;
                left++;
            }
            return left;

        }
        private int AvailableSpaceForRight(List<Point> list, Point p, Board knowBoard, int size)
        {
            int right = 0;
            Point h = new Point() { X = p.X, Y = p.Y };
            for (int i = 0; i < size && h.X < 9; i++)
            {
                h.X++;
                if (!list.Contains(new Point() { X = h.X, Y = p.Y })) break;

                right++;
            }
            return right;

        }
        private int AvailableSpaceForDown(List<Point> list, Point p, Board knowBoard, int size)
        {
            int down = 0;
            Point h = new Point() { X = p.X, Y = p.Y };
            for (int i = 0; i < size && h.Y < 9; i++)
            {
                h.Y++;
                if (!list.Contains(new Point() { X = h.X, Y = p.Y })) break;

                down++;
            }
            return down;

        }
        private int AvailableSpaceForUp(List<Point> list, Point p, Board knowBoard, int size)
        {
            int up = 0;
            Point h = new Point() { X = p.X, Y = p.Y };
            for (int i = 0; i < size && h.Y > 0; i++)
            {
                h.Y--;
                if (!list.Contains(new Point() { X = h.X, Y = p.Y })) break;

                up++;
            }
            return up;
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using Tecgraf.Battleship.Core;
using UnityEngine;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;

namespace Tecgraf.Battleship.Strategies
{

    public class SimpleBombardmentStrategy : IBombardmentStrategy
    {
        protected int lastX, lastY;
        protected struct Point
        {
            public int X;
            public int Y;
        };
        protected List<Point> bombardedPositions = new List<Point>();
        protected List<Point> possibleBombardmentPoints = new List<Point>();
        protected List<Point> ShipsToShot = new List<Point>();
        protected List<IShip> allShips = new List<IShip>() {
            new ShipDestroyer(), new ShipDestroyer(), new ShipDestroyer(), new ShipDestroyer(),
            new ShipCruiser(),new ShipCruiser(),new ShipCruiser(),
            new ShipBattleship(),new ShipBattleship(),
            new ShipCarrier(),
        };
        private ShipBombardmentDirection shipOrientation = ShipBombardmentDirection.Uknown;


        public void Bombard(Board knownBoard, out int x, out int y)
        {

            //Caso afundou um navio
            if (knownBoard.GetInfo(lastX, lastY) is IShip)
            {
                
                List<Point> ShipsToShotClone;
                ShipsToShotClone = new List<Point>(ShipsToShot);

                shipOrientation = ShipBombardmentDirection.Uknown;
                possibleBombardmentPoints.Clear();
                int i = 1;
                foreach(Point p in ShipsToShotClone)
                {
                    if(knownBoard.GetInfo(p.X,p.Y) is IShip)
                    {
                        ShipsToShot.Remove(p);
                        i++;
                    }
                }
                foreach (IShip ship in allShips)
                {
                    if(ship.Size == i)
                    {
                        allShips.Remove(ship);
                        break;
                    }
                }
                GenerateShotsBySucessfulShots(knownBoard);

            }
            //Caso acertou Agua
            if (knownBoard.GetInfo(lastX, lastY) is EmptyBoardCell)
            {
                if(ShipsToShot.Count != 0 && possibleBombardmentPoints.Count == 0)
                {
                    shipOrientation = ShipBombardmentDirection.Uknown;
                    GenerateShotsBySucessfulShots(knownBoard);
                }

            }
            //Caso acertou navio
            if (knownBoard.GetInfo(lastX, lastY) is OccupiedBoardCell)
            {
                ShipsToShot.Add(new Point() { X = lastX, Y = lastY });
                List<Point> nearShotedPoints = GeneratePossibleBombardmentPositions(lastX, lastY, false,knownBoard);
                ShipBombardmentDirection testShipOrientation = ShipBombardmentDirection.Uknown;
                foreach (Point p in nearShotedPoints)
                {
                    if (knownBoard.GetInfo(p.X, p.Y) is OccupiedBoardCell)
                    {
                        testShipOrientation = CalculateShipDirection(new Point() { X = lastX,Y = lastY},p);
                        if (testShipOrientation != ShipBombardmentDirection.Uknown)
                        {
                            shipOrientation = testShipOrientation;
                            break;
                        }
                    }
                }
                
                possibleBombardmentPoints = GeneratePossibleBombardmentPositions(lastX, lastY,true,knownBoard);
               

            }
            //Random
            if (possibleBombardmentPoints.Count == 0 && ShipsToShot.Count == 0)//Caso que não saiba de nada
            {
                shipOrientation = ShipBombardmentDirection.Uknown;

                DefaultBombard(knownBoard, out x, out y);
               // Debug.Log("Random");
            }
            //Caso seja possivel focar em um alvo
            else if(possibleBombardmentPoints.Count == 0 && ShipsToShot.Count != 0)
            {
               List<Point> ShipsToShotClone = new List<Point>(ShipsToShot);
                do
                {
                    Point p = new Point() { X = ShipsToShotClone[0].X, Y = ShipsToShotClone[0].Y };
                    possibleBombardmentPoints = GeneratePossibleBombardmentPositions(p.X, p.Y, true, knownBoard);
                    ShipsToShotClone.RemoveAt(0);
                } while (possibleBombardmentPoints.Count == 0 && ShipsToShotClone.Count!= 0);
                if (possibleBombardmentPoints.Count == 0)
                {
                    DefaultBombard(knownBoard, out x, out y);
                }
                else
                {
                    int j = Random.Range(0, possibleBombardmentPoints.Count);
                    x = possibleBombardmentPoints[j].X;
                    y = possibleBombardmentPoints[j].Y;
                    possibleBombardmentPoints.RemoveAt(j);
                }
            }
            //Caso tenha acertado um navio recentemente(ultimas 3 jogadas)
            else 
            {
                int j = Random.Range(0, possibleBombardmentPoints.Count);
                x = possibleBombardmentPoints[j].X;
                y = possibleBombardmentPoints[j].Y;
                possibleBombardmentPoints.RemoveAt(j);
                //Debug.Log("Calculado");
            }
            #region Todo turno

            EndTurn(x,y);
            #endregion
        }
        #region Auxiliares
        public virtual void EndTurn(int x, int y)
        {
            lastX = x;
            lastY = y;
            bombardedPositions.Add(new Point() { X = lastX, Y = lastY });
        }
        /// <summary>
        /// Generate a list of points to be targeted by player, based on next squads and a Point
        /// </summary>
        private List<Point> GeneratePossibleBombardmentPositions(int x, int y, bool filter,Board knownBoard)
        {
            List<Point> generatedList = new List<Point>();
            List<Point> generatedListClone = new List<Point>();
            if (shipOrientation != ShipBombardmentDirection.Vertical)
            {
                if (x > 0) generatedList.Add(new Point() { X = x - 1, Y = y });
                if (x < 9) generatedList.Add(new Point() { X = x + 1, Y = y });
            }
            if (shipOrientation != ShipBombardmentDirection.Horizontal)
            {
                if (y > 0) generatedList.Add(new Point() { X = x, Y = y - 1 });
                if (y < 9) generatedList.Add(new Point() { X = x, Y = y + 1 });
            }
            if (filter)
            {
                generatedListClone = new List<Point>(generatedList);
                foreach (Point p in generatedListClone)
                {
                    if (bombardedPositions.Contains(p)|| !AvailabeSpaceForShot(p,knownBoard,allShips[0].Size))
                    {
                        generatedList.Remove(p);
                    }

                }
            }
            else
            {
                foreach (Point p in generatedList)
                {
                    if (bombardedPositions.Contains(p))
                    {
                        generatedListClone.Add(new Point() { X = p.X, Y = p.Y });
                    }
                }
                return generatedListClone;
            }
            return generatedList;
        }
        /// <summary>
        /// Check if all elements of the list have the same BoardCellState
        /// </summary>
        private bool IsBoardCellList(List<Point> list,BoardCellState boardCell,Board knowBoard)
        {
            foreach(Point p in list)
            {
                if(knowBoard.GetCellState(p.X,p.Y) != boardCell)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Calculate the probably ship direction comparing two points
        /// </summary>
        private ShipBombardmentDirection CalculateShipDirection(Point p1,Point p2)
        {
            ShipBombardmentDirection tempShipOrientation;
            int tempX = p1.X - p2.X;
            int tempY = p1.Y - p2.Y;
            if (tempX == 0 && Mathf.Abs(tempY) == 1) tempShipOrientation = ShipBombardmentDirection.Vertical;
            else if (tempY == 0 && Mathf.Abs(tempX) == 1) tempShipOrientation = ShipBombardmentDirection.Horizontal;
            else tempShipOrientation = ShipBombardmentDirection.Uknown;
            return tempShipOrientation;
        }
        /// <summary>
        /// Check an available point in the list of ships hitted and start to bombard from that point
        /// </summary>
        private void GenerateShotsBySucessfulShots(Board knownBoard)
        {
            if(ShipsToShot.Count != 0)
            {
                List<Point> ShipsToShotClone = new List<Point>(ShipsToShot);
                List<Point> list = new List<Point>();
                
                do
                {
                    Point p = ShipsToShotClone[0];
                    if (AvailableSpaceInColumn(p, knownBoard, 2) && AvailableSpaceInRown(p, knownBoard, 1)) shipOrientation = ShipBombardmentDirection.Uknown;
                    else if (!AvailableSpaceInRown(p, knownBoard, 2)) shipOrientation = ShipBombardmentDirection.Vertical;
                    else shipOrientation = ShipBombardmentDirection.Horizontal;
                    list = GeneratePossibleBombardmentPositions(p.X, p.Y, true,knownBoard);

                    ShipsToShotClone.RemoveAt(0);
                } while (list.Count == 0 && ShipsToShotClone.Count !=0);
                possibleBombardmentPoints = list;
            }
        }
        /// <summary>
        /// Random Bombard a point
        /// </summary>
        protected virtual void DefaultBombard(Board knownBoard, out int x, out int y)
        {
            do
            {
                x = Random.Range(0, knownBoard.GridSize);
                y = Random.Range(0, knownBoard.GridSize);
                if (knownBoard.GetInfo(x, y) == null)
                {
                    Point p = new Point() { X = x, Y = y };
                    int size = allShips[allShips.Count - 1].Size;
                    if (AvailableSpaceInRown(p,knownBoard,size))
                    {
                        int right = AvailableSpaceForRight(p, knownBoard, size), left =  -1*AvailableSpaceForLeft(p, knownBoard, size);
                       
                        if ((left != 0 && right == 0) || ((right != 0 && left == 0)))
                        {
                            p.X += (left + right) / 2;
                            if(knownBoard.GetCellState(p.X,p.Y) == BoardCellState.Unknown && p.X >= 0 && p.X <= 9)
                            {
                                x = p.X;
                            }
                        }
                        break;
                    }
                    else if(AvailableSpaceInColumn(p, knownBoard, size))
                    {
                        int up = -1 *AvailableSpaceForUp(p, knownBoard, size), down = AvailableSpaceForDown(p, knownBoard, size);
                        if ((up != 0 && down == 0) || ((down != 0 && up == 0)))
                        {
                            p.Y = (up + down) / 2;
                            if (knownBoard.GetCellState(p.X, p.Y) == BoardCellState.Unknown && p.Y >= 0 && p.Y <= 9)
                            {
                                y = p.Y;
                            }
                        }
                        break;
                    }
                }
            }
            while (true);
        }
        /// <summary>
        /// Checks if a ship can be in that point by available points around
        /// </summary>
        protected bool AvailableSpaceForShip(Point p,Board knownBoard,int size)
        {
            
            if (AvailableSpaceInRown(p, knownBoard, size) || AvailableSpaceInColumn(p, knownBoard, size)){
                return true;
            }
            return false;
        }
        private bool AvailableSpaceInRown(Point p,Board knowBoard, int size)
        {
            if (AvailableSpaceForLeft(p, knowBoard, size) + AvailableSpaceForRight(p, knowBoard, size) + 1 < size) return false;
            return true;
        }
        private bool AvailableSpaceInColumn(Point p, Board knowBoard,int size)
        {
            if (AvailableSpaceForUp(p,knowBoard,size) + AvailableSpaceForDown( p,knowBoard,size) + 1 < size ) return false;
            return true;
        }
        private bool AvailabeSpaceForShot(Point p,Board knownBoard,int size)
        {
            if (AvailableSpaceInRown(p, knownBoard, size) || AvailableSpaceInColumn(p, knownBoard, size)) return true;
            return false;

        }


        private int AvailableSpaceForLeft(Point p, Board knowBoard, int size)
        {
            int left = 0;
            Point h = new Point() { X = p.X, Y = p.Y };
            for (int i = 0; i < size && h.X > 0; i++)
            {
                h.X--;
                if ((knowBoard.GetInfo(h.X, h.Y) is IShip) || knowBoard.GetInfo(h.X, h.Y) is EmptyBoardCell) break;

                left++;
            }
            return left;

        }
        private int AvailableSpaceForRight(Point p, Board knowBoard, int size)
        {
            int right = 0;
            Point h = new Point() { X = p.X, Y = p.Y };
            for (int i = 0; i < size && h.X < 9; i++)
            {
                h.X++;
                if ((knowBoard.GetInfo(h.X, h.Y) is IShip) || knowBoard.GetInfo(h.X, h.Y) is EmptyBoardCell) break;

                right++;
            }
            return right;

        }
        private int AvailableSpaceForDown(Point p, Board knowBoard, int size)
        {
            int down = 0;
            Point h = new Point() { X = p.X, Y = p.Y };
            for (int i = 0; i < size && h.Y < 9; i++)
            {
                h.Y++;
                if (knowBoard.GetInfo(h.X, h.Y) is IShip || knowBoard.GetInfo(h.X, h.Y) is EmptyBoardCell) break;

                down++;
            }
            return down;

        }
        private int AvailableSpaceForUp(Point p, Board knowBoard, int size)
        {
            int up = 0;
            Point h = new Point() { X = p.X, Y = p.Y };
            for (int i = 0; i < size && h.Y > 0; i++)
            {
                h.Y--;
                if (knowBoard.GetInfo(h.X, h.Y) is IShip || knowBoard.GetInfo(h.X, h.Y) is EmptyBoardCell) break;

                up++;
            }
            return up;
        }

        #endregion


    }
    #region Enums
    public enum ShipBombardmentDirection
    {
        Horizontal,
        Vertical,
        Uknown
    }
   
    #endregion
}

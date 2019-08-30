using System.Collections.Generic;

using Tecgraf.Battleship.Core;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;
using UnityEngine;

namespace Tecgraf.Battleship.Strategies
{
    public class FramePlacementStrategy : IPlacementStrategy
    {
        private struct Point
        {
            public int X;
            public int Y;
        };
        List<Point> framePoints = new List<Point>();
        public int IndividualRetryCount { get; set; } = 20;


        public void PlaceShips(Board board,List<IShip> ships)
        {
            bool success =false;
            if(framePoints.Count == 0)
            {
                OnStart();
            }
          
            while (!success) {
                success = true;
                foreach(var ship in ships)
                {
                    int[] bucket = new int[] { 0, 9 };
                    int maxCoordX = -1, maxCoordY = -1;
                    var orientation = (ShipPlacementOrientations)(Random.Range((int)0, (int)2));

                    if (orientation == ShipPlacementOrientations.Horizontal) maxCoordX = board.GridSize - ship.Size;
                    else maxCoordY = board.GridSize - ship.Size;
                    
                    for (int i = 0; IndividualRetryCount < 0 || i < IndividualRetryCount; i++)
                    {
                        int coordY = maxCoordY, coordX = maxCoordX;
                        if (coordX == -1)
                        {
                            if (board.PlaceShip(ship, orientation, bucket[Random.Range(0, 2)], Random.Range(0, coordY))) break;
                        }
                        else if (coordY == -1)
                        {
                            if(board.PlaceShip(ship, orientation, Random.Range(0, coordX), bucket[Random.Range(0, 2)])) break;
                        }
                        else
                        {
                            if (board.PlaceShip(ship, orientation, Random.Range(0, maxCoordX), Random.Range(0, maxCoordY)))
                                break;
                        }
                        if (i == IndividualRetryCount - 1)
                        {
                            board.Clear();
                            success = false;
                        }
                        if (!success)
                            break;
                    }




                }


            }



        }

        private void OnStart()
        {
            for(int i = 0;i < 10; i++)
            {
                framePoints.Add(new Point() { X = i, Y = 0 });
                framePoints.Add(new Point() { X = 0 , Y = i });
                framePoints.Add(new Point() { X = 9, Y = i});
                framePoints.Add(new Point() { X = i, Y = 9});
            }
        }
    }
}
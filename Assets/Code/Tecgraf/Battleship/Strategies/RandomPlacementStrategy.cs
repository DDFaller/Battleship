using System.Collections.Generic;

using Tecgraf.Battleship.Core;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;
using UnityEngine;

namespace Tecgraf.Battleship.Strategies
{
    public class RandomPlacementStrategy : IPlacementStrategy
    {
        public int OverallRetryCount { get; set; } = -1;
        public int IndividualRetryCount { get; set; } = 20;
        private struct Point {
            public int X;
            public int Y;
        }
        private List<Point> AvailablePositions = new List<Point>();
      
        public void PlaceShips( Board board, List<IShip> ships )
        {
            bool success = false;
            int attempts = 0;
         
            while( !success && ( attempts < OverallRetryCount || OverallRetryCount < 0 ) )
            { 
                success = true;
                attempts++;

                foreach( var ship in ships )
                {
                    var orientation = (ShipPlacementOrientations)( Random.Range( (int)0, (int)2 ) );

                    int maxCoordX = board.GridSize - ( orientation == ShipPlacementOrientations.Horizontal ? ship.Size : 0 );
                    int maxCoordY = board.GridSize - ( orientation == ShipPlacementOrientations.Vertical ? ship.Size : 0 );

                    for( int i = 0; IndividualRetryCount < 0 || i < IndividualRetryCount; i++ )
                    {

                        int coordX = Random.Range(0, maxCoordX);
                        int coordY = Random.Range(0, maxCoordY);
                        if ( board.PlaceShip( ship, orientation, coordX, coordY ))
                        {
                            break;
                        }
                            
                       
                        if( i == IndividualRetryCount - 1 )
                        {
                            board.Clear();
                         
                            success = false;
                        }
                    }

                    if( !success )
                        break;
                }
            }
        }
       
    }
}
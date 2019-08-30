using System.Linq;
using System.Collections.Generic;

using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;

namespace Tecgraf.Battleship.Core
{
    public class Board
    {
        private IBoardEntry[,] _grid;

        public int GridSize { get; private set; }

        public Board( int gridSize )
        {
            GridSize = gridSize;
            _grid = new IBoardEntry[GridSize, GridSize];
        }

        public IBoardEntry GetInfo( int x, int y )
        {
            return _grid[x, y];
        }

        public BoardCellState GetCellState( int x, int y )
        {
            if (x < 0 || y < 0 || x >= _grid.GetLength(0) || y >= _grid.GetLength(1))
                return BoardCellState.Invalid;

            if (_grid[x, y] == null) return BoardCellState.Unknown;

            if( _grid[x, y].IsEmpty ) return BoardCellState.Empty;

            return (_grid[x, y] is IShip ? BoardCellState.KnownShip : BoardCellState.Occupied);
        }

        public IBoardEntry Bombard( int x, int y )
        {   
            // Return result, obfuscate information about ship unless it was destroyed
            IShip ship = _grid[x, y] as IShip;
            if( ship != null )
            {
                ship.TakeDamage();
                if( ship.HitPoints == 0 )
                    return ship;

                return new OccupiedBoardCell() { X = x, Y = y };
            }

            return new EmptyBoardCell() { X = x, Y = y };
        }

        public void Clear()
        {
            _grid = new IBoardEntry[GridSize, GridSize];
        }

        public void SetInfo( int x, int y, IBoardEntry hit )
        {
            _grid[x, y] = hit;
        }

        public bool PlaceShip( IShip ship, ShipPlacementOrientations orientation, int x, int y )
        {
            if( !CheckValidPlacement( ship, orientation, x, y ) )
                return false;

            ApplyPlacement( ship, orientation, x, y );
            ship.Orientation = orientation;
            ship.X = x;
            ship.Y = y;

            return true;
        }

        public List<IShip> Ships
        {
            get
            {
                HashSet<IShip> ships = new HashSet<IShip>(){};

                for( int i = 0; i < _grid.GetLength( 0 ); i++ )
                {
                    for( int j = 0; j < _grid.GetLength( 1 ); j++ )
                    {
                        ships.Add( _grid[i, j] as IShip );
                    }
                }

                ships.Remove( null );
                return ships.ToList();
            }
        }

        public void ReplaceInformation( IShip ship )
        {
            ApplyPlacement( ship, ship.Orientation, ship.X, ship.Y );
        }

        public void RemoveShip( IShip ship )
        {
            for( int i = 0; i < _grid.GetLength( 0 ); i++ )
            {
                for( int j = 0; j < _grid.GetLength( 1 ); j++ )
                {
                    if( _grid[i, j] == ship )
                        _grid[i, j] = null;
                }
            }
        }

        private bool CheckValidPlacement( IShip ship, ShipPlacementOrientations orientation, int x, int y )
        {
            for( int i = 0; i < ship.Size; i++ )
            {
                // get x,y coordinates based on ship orientation
                int xi = x + ( orientation == ShipPlacementOrientations.Horizontal ? i : 0 );
                int yi = y + ( orientation == ShipPlacementOrientations.Vertical ? i : 0 );

                // check if out of bounds
                if( xi < 0 || xi > _grid.GetLength( 0 ) || yi < 0 || yi > _grid.GetLength( 1 ) )
                    return false;

                // check for collision with another ship
                if( _grid[xi, yi] is IShip )
                    return false;
            }

            return true;
        }

        private void ApplyPlacement( IShip ship, ShipPlacementOrientations orientation, int x, int y )
        {
            for( int i = 0; i < ship.Size; i++ )
            {
                
                // get x,y coordinates based on ship orientation
                int xi = x + ( orientation == ShipPlacementOrientations.Horizontal ? i : 0 );
                int yi = y + ( orientation == ShipPlacementOrientations.Vertical ? i : 0 );

                
                _grid[xi, yi] = ship;
            }
        }
    }
}
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;

namespace Tecgraf.Battleship.Core
{
    public class TrackerBoard
    {
        private bool[,] _grid;

        public TrackerBoard( int gridSize )
        {
            _grid = new bool[gridSize, gridSize];
        }


    }
}
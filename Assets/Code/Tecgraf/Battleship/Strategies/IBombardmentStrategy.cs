using Tecgraf.Battleship.Core;

namespace Tecgraf.Battleship.Strategies
{
    public interface IBombardmentStrategy
    {
        /// <summary>
        /// This method receives the known information about the opponent's board and returns a position to bombard.
        /// </summary>
        /// <param name="knownBoard">The board with known information. The unknown positions will be null values, the known positions will depend, for each position the x,y coordinate value will be EmptyBoardCell if a cell was bombarded and hit nothing, or OccupiedBoardCell if the bombardment hit something. Once an opponent's ship is destroyed, the OccupiedBoardCells will be replaced with that ship.</param>
        /// <param name="x">Returns the X portion of the next bombardment attempt coordinates.</param>
        /// <param name="y">Returns the Y portion of the next bombardment attempt coordinates.</param>
        void Bombard( Board knownBoard, out int x, out int y );
    }
}
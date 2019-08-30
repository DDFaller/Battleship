using System.Collections.Generic;
using Tecgraf.Battleship.Core;
using Tecgraf.Battleship.Domain.Ships;

namespace Tecgraf.Battleship.Strategies
{
    public interface IPlacementStrategy
    {
        void PlaceShips( Board board, List<IShip> ships );
    }
}
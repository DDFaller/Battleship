using System.Linq;
using System.Collections.Generic;

using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;
using Tecgraf.Battleship.Strategies;

namespace Tecgraf.Battleship.Core
{
    public class Player
    {
        public string Name { get; set; }
        public int ID { get; set; }

        public Board PlayerBoard { get; private set; }
        public Board KnownOpponentBoard { get; private set; }

        public IPlacementStrategy PlacementStrategy { get; set; }
        public IBombardmentStrategy BombardmentStrategy { get; set; }
        public List<IShip> Ships { get; set; }

        public int Attempts { get; set; }

        public bool IsAlive { get { return Ships.Any( s => s.HitPoints > 0 ); } }

        public Player( int gridSize )
        {
            PlayerBoard = new Board( gridSize );
            KnownOpponentBoard = new Board( gridSize );
            Attempts = 0;
        }

        public void PlaceShips()
        {
            PlayerBoard.Clear();
            PlacementStrategy.PlaceShips( PlayerBoard, Ships );
        }

        public void Bombard( Player opponent, out int x, out int y, out IBoardEntry hit )
        {
            Attempts++;

            BombardmentStrategy.Bombard( KnownOpponentBoard, out x, out y );
            hit = opponent.PlayerBoard.Bombard( x, y );
            KnownOpponentBoard.SetInfo( x, y, hit );

            if( hit is IShip )
                KnownOpponentBoard.ReplaceInformation( hit as IShip );
        }
    }
}
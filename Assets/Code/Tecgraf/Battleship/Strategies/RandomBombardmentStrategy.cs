    using UnityEngine;

using Tecgraf.Battleship.Core;

namespace Tecgraf.Battleship.Strategies
{
    public class RandomBombardmentStrategy : IBombardmentStrategy
    {
        public bool CanSkip(Board kb, int x, int y)
        {
            if (kb.GetCellState(x + 1, y) == BoardCellState.Unknown) return false;
            else if (kb.GetCellState(x - 1, y) == BoardCellState.Unknown) return false;
            else if (kb.GetCellState(x, y + 1) == BoardCellState.Unknown) return false;
            else if (kb.GetCellState(x, y - 1) == BoardCellState.Unknown) return false;
            else return true;
        }

        public void Bombard( Board knownBoard, out int x, out int y )
        {
            do
            {
                x = Random.Range( 0, knownBoard.GridSize );
                y = Random.Range( 0, knownBoard.GridSize );
            }
            while(CanSkip(knownBoard,x,y) || knownBoard.GetInfo( x, y ) != null );
        }
    }
}   
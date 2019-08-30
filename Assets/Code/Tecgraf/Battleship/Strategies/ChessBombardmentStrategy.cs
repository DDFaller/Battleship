using System.Collections;
using System.Collections.Generic;
using Tecgraf.Battleship.Core;
using UnityEngine;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;

namespace Tecgraf.Battleship.Strategies
{
    public class ChessBombardmentStrategy : SimpleBombardmentStrategy, IBombardmentStrategy {

        protected override void DefaultBombard(Board knownBoard, out int x, out int y)
        {
            do
                {
                    x = Random.Range(0, knownBoard.GridSize);
                    if(x % 2 == 0){
                        y = Random.Range(0, knownBoard.GridSize/2) * 2;
                    }
                    else
                    {
                        y = Random.Range(0, knownBoard.GridSize / 2) * 2 + 1;
                    }
                    if (knownBoard.GetInfo(x, y) == null)
                    {
                    if (AvailableSpaceForShip(new Point() { X = x, Y = y }, knownBoard,allShips[allShips.Count -1].Size))
                    {
                        break;
                    }
                    }
                }
                while (true);
        }







    }




}
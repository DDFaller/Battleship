using System.Collections;
using System.Collections.Generic;
using Tecgraf.Battleship.Core;
using UnityEngine;

namespace Tecgraf.Battleship.Strategies
{

    public class DanielBombardmentStrat : IBombardmentStrategy
    {
        private int lastX = -1,lastY = 0;
        public void Bombard(Board knownBoard, out int x, out int y)
        {
            x = lastX;
            y = lastY;
            if(x + 1 < knownBoard.GridSize)
            {
                x++;
            }
            else
            {
                x = 0;
                y++;
            }
            lastX = x;
            lastY = y;
        }
    }
}

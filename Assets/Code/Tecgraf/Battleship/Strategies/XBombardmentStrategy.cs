using System.Collections;
using System.Collections.Generic;
using Tecgraf.Battleship.Core;
using UnityEngine;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;

namespace Tecgraf.Battleship.Strategies
{
    public class XBombardmentStrategy : SimpleBombardmentStrategy, IBombardmentStrategy
    {
        private List<Point> diagonal = new List<Point>();
        private bool start = true;

        protected override void DefaultBombard(Board knownBoard, out int x, out int y)
        {
            if (start)
            {
                start = false;
                OnStart();
            }
            while (diagonal.Count > 0 && !AvailableSpaceForShip(new Point() {X = diagonal[0].X,Y = diagonal[0].Y },knownBoard,allShips[allShips.Count -1].Size) )
            {
                diagonal.RemoveAt(0);
            }
            if (diagonal.Count != 0)
            {
                x = diagonal[0].X;
                y = diagonal[0].Y;
                diagonal.RemoveAt(0);
            }
            else
            {
                base.DefaultBombard(knownBoard,out x,out y);
            }
        }
        public override void EndTurn(int x, int y)
        {
            base.EndTurn(x, y);
            if(diagonal.Contains(new Point() { X = x, Y = y }))
            {
                diagonal.Remove(new Point() { X = x, Y = y });
            }
        }
        private void OnStart()
        {
           
            
            for(int i = 1; i<=2;i++)
            {
                diagonal.Add(new Point() { X = i - 1 , Y = 2 -i  });
                diagonal.Add(new Point() { X = 7 + i , Y = 10 -i });
            }

            for(int i = 0;i < 6; i++)
            {
                diagonal.Add(new Point() {X = i  ,Y =6 - i});
            }

           
            for (int x = 0; x < 10; x++)
            {
                diagonal.Add(new Point() { X = x, Y = 9 - x });
            }

            for (int i = 0; i < 6; i++)
            {
                diagonal.Add(new Point() { X = i, Y = 6 - i });
            }
            for(int i =4; i < 10; i++)
            {
                diagonal.Add(new Point() {X = i, Y = 9- (i -4) });
            }



        }

    }
}
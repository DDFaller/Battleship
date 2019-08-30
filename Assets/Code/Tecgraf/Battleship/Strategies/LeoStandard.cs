using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tecgraf.Battleship.Core;
using Tecgraf.Battleship.Domain.Ships;

namespace Tecgraf.Battleship.Strategies
{
    public class LeoStandard : IBombardmentStrategy
    {
        private int _lastX, _lastY;
        private List<Vector2> _positions;
        private List<Vector2> _hits;
        private Direction dir = Direction.Unknown;

        public LeoStandard()
        {
            _positions = new List<Vector2>();
            _hits = new List<Vector2>();
        }

        private void FillPosition(Board knownBoard, int x, int y)
        {
            if (knownBoard.GetCellState(x, y) == BoardCellState.Unknown)
                _positions.Add(new Vector2(x, y));
        }

        private void FillAll(Board kb, int x, int y)
        {
            if (dir == Direction.Unknown)
            {
                FillPosition(kb, x + 1, y);
                FillPosition(kb, x - 1, y);
                FillPosition(kb, x, y + 1);
                FillPosition(kb, x, y - 1);
            }
            else if (dir == Direction.Horizontal)
            {
                FillPosition(kb, x + 1, y);
                FillPosition(kb, x - 1, y);
            }
            else
            {
                FillPosition(kb, x, y + 1);
                FillPosition(kb, x, y - 1);
            }
        }

        public void RandomBombard(Board knownBoard, out int x, out int y)
        {
            do
            {
                x = Random.Range(0, knownBoard.GridSize);
                if (x % 2 == 0)
                {
                    y= Random.Range(0, knownBoard.GridSize/2)*2;
                }
                else
                {
                    y = Random.Range(0, knownBoard.GridSize / 2) * 2+1;
                }
            }
            while (knownBoard.GetCellState(x, y) != BoardCellState.Unknown);
        }

        private enum Direction
        {
            Unknown,
            Horizontal,
            Vertical
        }

        private bool isClose(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && (y1 - y2 == 1 || y1 - y2 == -1)) return true;
            if (y1 == y2 && (x1 - x2 == 1 || x1 - x2 == -1)) return true;
            else return false;
        }

        private Direction FindDirection(int x1, int x2)
        {

            if (x1 == x2) return Direction.Vertical;
            else return Direction.Horizontal;
        }

        public bool CanSkip(Board kb, int x, int y)
        {
            if (kb.GetCellState(x + 1, y) == BoardCellState.Unknown) return false;
            else if (kb.GetCellState(x - 1, y) == BoardCellState.Unknown) return false;
            else if (kb.GetCellState(x, y + 1) == BoardCellState.Unknown) return false;
            else if (kb.GetCellState(x, y - 1) == BoardCellState.Unknown) return false;
            else return true;
        }


        public void Bombard(Board knownBoard, out int x, out int y)
        {
            //acertou
            if (knownBoard.GetCellState(_lastX, _lastY) == BoardCellState.Occupied)
            {
                _hits.Add(new Vector2(_lastX, _lastY));
                FillAll(knownBoard, _lastX, _lastY);
            }

            //afundou
            if (knownBoard.GetInfo(_lastX, _lastY) is IShip)
            {
                _positions.Clear();
                for(int i = 0; i < _hits.Count; i++)
                {
                    if (knownBoard.GetInfo((int)_hits[i].x, (int)_hits[i].y) is IShip)
                    {
                        _hits.RemoveAt(i);
                    }
                }
                for(int i = 0; i<_hits.Count; i++)
                {
                    
                    FillAll(knownBoard, (int)_hits[i].x, (int)_hits[i].y);
                }
            }

            //decide valores de X e Y
            do
            {
                if (_positions.Count > 0)
                {
                    x = (int)_positions[0].x;
                    y = (int)_positions[0].y;
                    _positions.RemoveAt(0);
                }
                else
                {
                    do
                    {
                        RandomBombard(knownBoard, out x, out y);
                    }
                    while (CanSkip(knownBoard, x, y));
                }
            } while (knownBoard.GetCellState(x, y) != BoardCellState.Unknown );

            
            
            _lastX = x;
            _lastY = y;
        }

    }
}

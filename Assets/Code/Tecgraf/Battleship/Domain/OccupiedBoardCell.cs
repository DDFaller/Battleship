namespace Tecgraf.Battleship.Domain
{
    public class OccupiedBoardCell : IBoardEntry
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsEmpty { get { return false; } }
    }
}
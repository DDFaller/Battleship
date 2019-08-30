namespace Tecgraf.Battleship.Domain
{
    public class EmptyBoardCell : IBoardEntry
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsEmpty { get { return true; } }
    }
}
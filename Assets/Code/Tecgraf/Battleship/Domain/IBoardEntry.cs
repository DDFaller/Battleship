namespace Tecgraf.Battleship.Domain
{
    public interface IBoardEntry
    {
        int X { get; set; }
        int Y { get; set; }

        bool IsEmpty { get; }
    }
}
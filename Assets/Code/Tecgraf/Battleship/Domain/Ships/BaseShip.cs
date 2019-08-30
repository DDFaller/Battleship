namespace Tecgraf.Battleship.Domain.Ships
{
    public class BaseShip : IShip
    {
        public virtual string Name { get { return ""; } }
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsEmpty { get { return true; } }

        public virtual int Size { get { return 0; } }

        public int HitPoints { get; set; }

        public ShipPlacementOrientations Orientation { get; set; }

        public BaseShip()
        {
            HitPoints = Size;
        }

        public bool TakeDamage()
        {
            HitPoints--;
            return ( HitPoints > 0 );
        }
    }
}
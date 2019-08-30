namespace Tecgraf.Battleship.Domain.Ships
{
    public class ShipCruiser : BaseShip
    {
        public override string Name { get { return "Cruiser"; } }

        public override int Size { get { return 3; } }
    }
}
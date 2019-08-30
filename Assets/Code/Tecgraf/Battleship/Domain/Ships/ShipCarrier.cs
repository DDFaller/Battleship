namespace Tecgraf.Battleship.Domain.Ships
{
    public class ShipCarrier: BaseShip
    {
        public override string Name { get { return "Carrier"; } }

        public override int Size { get { return 5; } }
    }
}
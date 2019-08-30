namespace Tecgraf.Battleship.Domain.Ships
{
    public class ShipBattleship : BaseShip
    {
        public override string Name { get { return "Battleship"; } }

        public override int Size { get { return 4; } }
    }
}
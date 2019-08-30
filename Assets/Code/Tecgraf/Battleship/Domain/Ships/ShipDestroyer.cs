namespace Tecgraf.Battleship.Domain.Ships
{
    public class ShipDestroyer : BaseShip
    {
        public override string Name { get { return "Destroyer"; } }

        public override int Size { get { return 2; } }
    }
}
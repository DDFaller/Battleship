namespace Tecgraf.Battleship.Domain.Ships
{
    public interface IShip : IBoardEntry
    {
        /// <summary>
        /// Name of the ship.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Size of the ship. Ships are always 1 dimensional (no width, only length).
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Hit points of the ship (begins equal to size)
        /// </summary>
        int HitPoints { get; set; }

        /// <summary>
        /// Causes ship to take 1 point of damage.
        /// </summary>
        /// <returns>Returns if ship is still alive.</returns>
        bool TakeDamage();

        ShipPlacementOrientations Orientation { get; set; }
    }
}
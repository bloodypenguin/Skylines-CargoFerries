using System;

namespace CargoFerries.Config
{
    [Flags]
    public enum VehicleCategory
    {
        None = 0,
        CargoShip = 1,
        CargoHelicopter = 2,
        Ships = CargoShip,
        Helicopters = CargoHelicopter,
        All = Ships | Helicopters
    }
}

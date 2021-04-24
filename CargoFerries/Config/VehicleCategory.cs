using System;

namespace CargoFerries.Config
{
    [Flags]
    public enum VehicleCategory
    {
        None = 0,
        CargoShip = 1,
        Ships = CargoShip,
        All = Ships
    }
}

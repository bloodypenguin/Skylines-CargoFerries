using ICities;

namespace CargoFerries
{
    public class CargoFerriesMod : IUserMod
    {
        public static int MaxVehicleCount;
        
        public string Name => "Barges";
        public string Description => "Adds a new type of cargo transport - Barges. They are like cargo ships but use ferry paths & canals";
    }
}

using CargoFerries.OptionsFramework.Extensions;
using CitiesHarmony.API;
using ICities;

namespace CargoFerries
{
    public class CargoFerriesMod : IUserMod
    {
        public static int MaxVehicleCount;
        
        public string Name => "Barges - P&P Temp Fix";
        public string Description => "Adds a new type of cargo transport - Barges. They are like cargo ships but use ferry paths & canals";
        
        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>();
        }
        
        public void OnEnabled() {
            HarmonyHelper.EnsureHarmonyInstalled();
        }
    }
}

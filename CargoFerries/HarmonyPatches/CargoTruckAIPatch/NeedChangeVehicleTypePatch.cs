using System.Reflection;
using CargoFerries.Utils;

namespace CargoFerries.HarmonyPatches.CargoTruckAIPatch
{
    internal static class NeedChangeVehicleTypePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(CargoTruckAI), nameof(CargoTruckAI.NeedChangeVehicleType),
                    BindingFlags.Public | BindingFlags.Static),
                null, null,
                new PatchUtil.MethodDefinition(typeof(VehicleTypeReplacingTranspiler), (nameof(VehicleTypeReplacingTranspiler.Transpile))));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(CargoTruckAI),
                nameof(CargoTruckAI.NeedChangeVehicleType),
                BindingFlags.Public | BindingFlags.Static));
        }


    }
}
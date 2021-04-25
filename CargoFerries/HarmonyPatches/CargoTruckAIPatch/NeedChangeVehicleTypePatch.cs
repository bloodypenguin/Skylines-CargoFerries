using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CargoFerries.HarmonyPatches.CargoTruckAIPatch.Transpiler;
using CargoFerries.Utils;
using Harmony;
using UnityEngine;

namespace CargoFerries.HarmonyPatches.CargoTruckAIPatch
{
    public static class NeedChangeVehicleTypePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(CargoTruckAI), nameof(CargoTruckAI.NeedChangeVehicleType),
                    BindingFlags.Public | BindingFlags.Static),
                null, null,
                new PatchUtil.MethodDefinition(typeof(NeedChangeVehicleTypePatch), (nameof(VehicleTypeReplacingTranspiler.Transpile))));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(CargoTruckAI),
                nameof(CargoTruckAI.NeedChangeVehicleType),
                BindingFlags.Public | BindingFlags.Static));
        }


    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CargoFerries.Utils;
using ColossalFramework;
using HarmonyLib;
using UnityEngine;

namespace CargoFerries.HarmonyPatches.CargoTruckAIPatch
{
    internal static class ChangeVehicleTypePatch
    {

        private static bool isApplied = false;
        
        public static void Apply()
        {
            if (isApplied)
            {
                return;
            }
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(global::CargoTruckAI), nameof(global::CargoTruckAI.ChangeVehicleType),
                    BindingFlags.Static | BindingFlags.Public),
                null, null,
                new PatchUtil.MethodDefinition(typeof(ChangeVehicleTypePatch), (nameof(Transpile))));
            isApplied = true;

        }

        public static void Undo()
        {
            if (!isApplied)
            {
                return;
            }
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(global::CargoTruckAI),
                nameof(global::CargoTruckAI.ChangeVehicleType),
                BindingFlags.Static | BindingFlags.Public));
            isApplied = false;
        }

        private static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Barges: Transpiling method: " + original.DeclaringType + "." + original);
            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            foreach (var codeInstruction in codes)
            {
                if (SkipInstruction(codeInstruction))
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }

                var patchIndex = newCodes.Count - 9;
                newCodes.RemoveRange(patchIndex, 2); //remove randomizer
                newCodes.Insert(patchIndex, new CodeInstruction(OpCodes.Ldloc_S, 6));
                newCodes.Insert(patchIndex + 1, new CodeInstruction(OpCodes.Ldloc_S, 7));
                newCodes.Add(new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(ChangeVehicleTypePatch), nameof(GetCargoVehicleInfo))));
                Debug.Log(
                    "Barges: Transpiled CargoTruckAI.ChangeVehicleType()");
            }

            return newCodes.AsEnumerable();
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.opcode != OpCodes.Callvirt || codeInstruction.operand == null ||
                   !codeInstruction.operand.ToString().Contains(nameof(VehicleManager.GetRandomVehicleInfo));
        }

        private static VehicleInfo GetCargoVehicleInfo(
            VehicleManager instance,
            ushort cargoStation1, ushort cargoStation2,
            ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            var infoFrom = BuildingManager.instance.m_buildings.m_buffer[cargoStation1].Info;
            if (infoFrom?.m_class?.name == ItemClasses.cargoFerryFacility.name) //to support Cargo Ferries
            {
                level = ItemClass.Level.Level5;
            }

            var vehicleInfo = instance.GetRandomVehicleInfo(
                ref Singleton<SimulationManager>.instance.m_randomizer, service, subService, level);
            if (vehicleInfo == null && infoFrom?.m_class?.name == ItemClasses.cargoFerryFacility.name)
            {
                UnityEngine.Debug.LogWarning("Barges: no barges found!");
            }
            return vehicleInfo;
        }
    }
}
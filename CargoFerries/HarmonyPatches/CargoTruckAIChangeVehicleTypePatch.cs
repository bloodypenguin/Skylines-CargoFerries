using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CargoFerries.Utils;
using ColossalFramework;
using Harmony;
using UnityEngine;

namespace CargoFerries.HarmonyPatches
{
    public class CargoTruckAIChangeVehicleTypePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(CargoTruckAI), nameof(CargoTruckAI.ChangeVehicleType),
                    BindingFlags.Static | BindingFlags.Public),
                null, null,
                new PatchUtil.MethodDefinition(typeof(CargoTruckAIChangeVehicleTypePatch), (nameof(Transpile))));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(CargoTruckAI),
                nameof(CargoTruckAI.ChangeVehicleType),
                BindingFlags.Static | BindingFlags.Public));
        }

        private static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("MCM: Transpiling method: " + original.DeclaringType + "." + original);
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
                    AccessTools.Method(typeof(CargoTruckAIChangeVehicleTypePatch), nameof(GetCargoVehicleInfo))));
                Debug.Log(
                    "MCM: Transpiled CargoTruckAI.ChangeVehicleType()");
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
                UnityEngine.Debug.LogWarning("No Cargo Ferries found!");
            }
            return vehicleInfo;
        }
    }
}
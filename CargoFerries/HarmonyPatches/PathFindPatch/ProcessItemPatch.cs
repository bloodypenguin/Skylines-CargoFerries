using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CargoFerries.Utils;
using HarmonyLib;
using UnityEngine;

namespace CargoFerries.HarmonyPatches.PathFindPatch
{
    internal class ProcessItemPatch
    {

        private static FieldInfo laneTypesField =
            typeof(PathFind).GetField("m_laneTypes", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo vehicleTypesField =
            typeof(PathFind).GetField("m_vehicleTypes", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PathFind), "ProcessItem",
                    argumentTypes: FindProcessItemMethodArgTypes()),
                null, null,
                new PatchUtil.MethodDefinition(typeof(ProcessItemPatch), (nameof(Transpile))));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(PathFind), "ProcessItem",
                argumentTypes: FindProcessItemMethodArgTypes()));
        }
        
        private static Type[] FindProcessItemMethodArgTypes()
        {
            var methodInfo = typeof(PathFind).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(m => m.Name == "ProcessItem" && m.GetParameters().Length == 5);
            if (methodInfo == null)
            {
                throw new Exception("PathFind.ProcessItem() with 5 args not found!");
            }
            UnityEngine.Debug.Log("Barges - PathFind.ProcessItem() with 5 args is found");
            return methodInfo.GetParameters().Select(p => p.ParameterType)
                .Select(type =>
                {
                    UnityEngine.Debug.Log(type);
                    return type;
                })
                .ToArray();
        }
        
        internal static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            var replaceNextWithNop = false;
            foreach (var codeInstruction in codes)
            {
                if (replaceNextWithNop)
                {
                    newCodes.Add(new CodeInstruction(OpCodes.Nop));
                    replaceNextWithNop = false;
                    continue;
                    
                }
                if (codeInstruction.opcode != OpCodes.Ldc_I4 || codeInstruction.operand is not 3072)
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }
                newCodes.RemoveAt(newCodes.Count - 1);
                newCodes.Add(new CodeInstruction(OpCodes.Ldarg_3));
                newCodes.Add(new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(ProcessItemPatch), nameof(TreatSpecially))));
                //because there will be 'and' that has to be removed
                replaceNextWithNop = true;
                Debug.Log($"Barges: PathFindPatch - prevented trucks from using train tracks");
            }
            return newCodes.AsEnumerable();
        }

        //prevents trucks from using train tracks. That issue happens because this mod adds Ferry to vehicleTypes
        //only trucks/post vans request CargoVehicle lane type so we can easily identify them
        private static bool TreatSpecially(PathFind pathFind, ref NetNode node)
        {
            var vehicleTypes =  (VehicleInfo.VehicleType) vehicleTypesField.GetValue(pathFind);
            var laneTypes = (NetInfo.LaneType) laneTypesField.GetValue(pathFind);
            return ((laneTypes & NetInfo.LaneType.CargoVehicle) == NetInfo.LaneType.None || BelongsToFerryNetwork(ref node))
                   && DoVanillaCheck(vehicleTypes);

            static bool DoVanillaCheck(VehicleInfo.VehicleType vehicleType)
            {
                return (vehicleType & (VehicleInfo.VehicleType.Ferry | VehicleInfo.VehicleType.Monorail)) !=
                       VehicleInfo.VehicleType.None;
            }
        }

        private static bool BelongsToFerryNetwork(ref NetNode node)
        {
            for (var index = 0; index < 8; ++index)
            {
                var segment = node.GetSegment(index);
                if (segment == 0)
                {
                    continue;
                }
                var segmentInfo = NetManager.instance.m_segments.m_buffer[segment].Info;
                if (segmentInfo == null)
                {
                    continue;
                }

                if ((segmentInfo.m_vehicleTypes & VehicleInfo.VehicleType.Ferry) == VehicleInfo.VehicleType.None)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
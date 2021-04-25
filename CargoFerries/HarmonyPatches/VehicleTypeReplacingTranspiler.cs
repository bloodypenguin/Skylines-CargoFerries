using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace CargoFerries.HarmonyPatches
{
    public static class VehicleTypeReplacingTranspiler
    {
        public static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Barges: VehicleTypeReplacingTranspiler - Transpiling method: " + original.DeclaringType + "." + original);
            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            foreach (var codeInstruction in codes)
            {
                if (SkipInstruction(codeInstruction))
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }
                var newInstruction = new CodeInstruction(OpCodes.Ldc_I4,  ((sbyte)28).Equals(codeInstruction.operand)
                        ? (int)(VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Ship | VehicleInfo.VehicleType.Plane | VehicleInfo.VehicleType.Ferry | VehicleInfo.VehicleType.Helicopter) 
                        : (int)(VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Ship | VehicleInfo.VehicleType.Plane | VehicleInfo.VehicleType.Ferry | VehicleInfo.VehicleType.Helicopter | VehicleInfo.VehicleType.Car))
                    {
                        labels = codeInstruction.labels
                    }
                    ;
                newCodes.Add(newInstruction);
                Debug.Log($"Barges: VehicleTypeReplacingTranspiler - Replaced vehicle type with {newInstruction.operand}");
            }

            return newCodes.AsEnumerable();
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.opcode != OpCodes.Ldc_I4_S || codeInstruction.operand == null ||
                   !(((sbyte) 28).Equals(codeInstruction.operand) || ((sbyte) 29).Equals(codeInstruction.operand));
        }
    }
}
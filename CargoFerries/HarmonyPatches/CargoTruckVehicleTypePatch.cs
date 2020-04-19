using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CargoFerries.Utils;
using Harmony;
using UnityEngine;

namespace CargoFerries.HarmonyPatches
{
    public class CargoTruckVehicleTypePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(CargoTruckAI), nameof(CargoTruckAI.NeedChangeVehicleType),
                    BindingFlags.Public | BindingFlags.Static),
                null, null,
                new PatchUtil.MethodDefinition(typeof(CargoTruckVehicleTypePatch), (nameof(Transpile))));
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(CargoTruckAI), "StartPathFind",
                    BindingFlags.Default, new[]
                    {
                        typeof(ushort),
                        typeof(Vehicle).MakeByRefType(),
                        typeof(Vector3),
                        typeof(Vector3),
                        typeof(bool),
                        typeof(bool),
                        typeof(bool)
                    }),
                null, null,
                new PatchUtil.MethodDefinition(typeof(CargoTruckVehicleTypePatch), (nameof(Transpile))));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(CargoTruckAI),
                nameof(CargoTruckAI.NeedChangeVehicleType),
                BindingFlags.Public | BindingFlags.Static));
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(CargoTruckAI), "StartPathFind",
                BindingFlags.Default, new[]
                {
                    typeof(ushort),
                    typeof(Vehicle).MakeByRefType(),
                    typeof(Vector3),
                    typeof(Vector3),
                    typeof(bool),
                    typeof(bool),
                    typeof(bool)
                }));
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
                var newInstuction = new CodeInstruction(OpCodes.Ldc_I4,  ((sbyte)28).Equals(codeInstruction.operand) ? ((int)1052) : ((int)1053))
                {
                    labels = codeInstruction.labels
                }
                ;
                newCodes.Add(newInstuction);
                Debug.LogWarning(
                    $"MCM: Replaced vehicle type with {newInstuction.operand}");
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
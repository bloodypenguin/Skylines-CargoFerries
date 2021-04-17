using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CargoFerries.Utils;
using Harmony;
using UnityEngine;

namespace CargoFerries.HarmonyPatches
{
    public class FerryAIPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(FerryAI), nameof(FerryAI.SimulationStep),
                    argumentTypes: new Type[] {typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(Vector3)}),
                null, null,
                new PatchUtil.MethodDefinition(typeof(FerryAIPatch), (nameof(Transpile))));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(FerryAI), nameof(FerryAI.SimulationStep),
                argumentTypes: new Type[] {typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(Vector3)}));
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

                var newInstruction = codeInstruction.operand.Equals(65796)
                        ?new CodeInstruction(OpCodes.Ldc_I4,  
                        (int)(Vehicle.Flags.Spawned | Vehicle.Flags.WaitingPath | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo))
                    {
                        labels = codeInstruction.labels
                    }
                        :new CodeInstruction(OpCodes.Ldc_I4, (int)byte.MaxValue)
                        {
                            labels = codeInstruction.labels
                        }
                    ;
                newCodes.Add(newInstruction);
                Debug.LogWarning(
                    $"MCM: Replaced vehicle flags with {newInstruction.operand}");
            }

            return newCodes.AsEnumerable();
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.opcode != OpCodes.Ldc_I4 || codeInstruction.operand == null ||
                   (!65796.Equals(codeInstruction.operand) && !150.Equals(codeInstruction.operand));
        }
    }
}

using System.Reflection;
using CargoFerries.Utils;
using UnityEngine;

namespace CargoFerries.HarmonyPatches.PostVanAIPatch
{
    public static class StartPathFindPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PostVanAI), "StartPathFind",
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
                new PatchUtil.MethodDefinition(typeof(VehicleTypeReplacingTranspiler), (nameof(VehicleTypeReplacingTranspiler.Transpile))));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(PostVanAI), "StartPathFind",
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
    }
}
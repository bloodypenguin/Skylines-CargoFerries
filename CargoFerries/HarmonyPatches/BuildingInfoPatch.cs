using System;
using System.Reflection;
using CargoFerries.AI;
using CargoFerries.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CargoFerries.HarmonyPatches
{
    public class BuildingInfoPatch
    {
        private static bool deployed;

        public static void Apply()
        {
            if (deployed)
            {
                return;
            }

            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(BuildingInfo), nameof(BuildingInfo.InitializePrefab)),
                new PatchUtil.MethodDefinition(typeof(BuildingInfoPatch), nameof(PreInitializePrefab)));

            deployed = true;
        }

        public static void Undo()
        {
            if (!deployed)
            {
                return;
            }

            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(BuildingInfo), nameof(BuildingInfo.InitializePrefab)));

            deployed = false;
        }

        private static bool PreInitializePrefab(BuildingInfo __instance)
        {
            try
            {
                if (__instance?.m_class?.name != ItemClasses.cargoFerryFacility.name)
                {
                    return true;
                }

                var oldAi = __instance.GetComponent<CargoHarborAI>();
                Object.DestroyImmediate(oldAi);
                var ai = __instance.gameObject.AddComponent<CargoFerryHarborAI>();
                PrefabUtil.TryCopyAttributes(oldAi, ai, false);
                typeof(CargoHarborAI).GetField("m_quayOffset", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(ai, -4f); //TODO: only patch my asset
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return true;
        }
    }
}
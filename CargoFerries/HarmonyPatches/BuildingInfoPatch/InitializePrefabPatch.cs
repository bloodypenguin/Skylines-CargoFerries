using System;
using CargoFerries.AI;
using CargoFerries.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CargoFerries.HarmonyPatches.BuildingInfoPatch
{
    
    public class InitializePrefabPatch
    {
        private static bool deployed;

        public static void Apply()
        {
            if (deployed)
            {
                return;
            }

            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(global::BuildingInfo), nameof(global::BuildingInfo.InitializePrefab)),
                new PatchUtil.MethodDefinition(typeof(InitializePrefabPatch), nameof(PreInitializePrefab)));

            deployed = true;
        }

        public static void Undo()
        {
            if (!deployed)
            {
                return;
            }

            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(global::BuildingInfo), nameof(global::BuildingInfo.InitializePrefab)));

            deployed = false;
        }

        private static bool PreInitializePrefab(global::BuildingInfo __instance)
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
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return true;
        }
    }
}
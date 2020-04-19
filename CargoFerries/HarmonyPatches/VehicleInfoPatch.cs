using System;
using System.Linq;
using CargoFerries.AI;
using CargoFerries.Config;
using CargoFerries.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CargoFerries.HarmonyPatches
{
    public class VehicleInfoPatch
    {
        private static bool deployed;

        public static void Apply()
        {
            if (deployed)
            {
                return;
            }

            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(VehicleInfo), nameof(VehicleInfo.InitializePrefab)),
                new PatchUtil.MethodDefinition(typeof(VehicleInfoPatch), nameof(PreInitializePrefab)));

            deployed = true;
        }

        public static void Undo()
        {
            if (!deployed)
            {
                return;
            }

            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(VehicleInfo), nameof(VehicleInfo.InitializePrefab)));

            deployed = false;
        }

        private static bool PreInitializePrefab(VehicleInfo __instance)
        {
            try
            {
                if (Util.TryGetWorkshopId( __instance, out long id) && Ships.GetConvertedIds(VehicleCategory.CargoShip).Contains(id))
                {
                    ConvertToCargoFerry(__instance);
                }


            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return true;
        }

        private static void ConvertToCargoFerry(VehicleInfo __instance)
        {
            if (__instance.m_class.m_subService == ItemClass.SubService.PublicTransportShip &&
                __instance.m_class?.m_level == ItemClass.Level.Level4 &&
                __instance.GetComponent<VehicleAI>() is CargoShipAI)
            {
                __instance.m_vehicleType = VehicleInfo.VehicleType.Ferry;
                __instance.m_class = ItemClasses.cargoFerryVehicle;
                var ai = ReplaceAI<CargoFerryAI>(__instance);
                ai.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry");
            }
        }

        private static T ReplaceAI<T>(VehicleInfo __instance) where T : VehicleAI
        {
            var oldAi = __instance.GetComponent<VehicleAI>();
            Object.DestroyImmediate(oldAi);
            var ai = __instance.gameObject.AddComponent<T>();
            PrefabUtil.TryCopyAttributes(oldAi, ai, false);
            ai.m_info = __instance;
            __instance.m_vehicleAI = ai;
            return ai;
        }
    }
}
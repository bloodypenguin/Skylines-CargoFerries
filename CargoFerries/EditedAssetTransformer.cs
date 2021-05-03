using CargoFerries.AI;
using CargoFerries.Utils;
using UnityEngine;

namespace CargoFerries
{
    public static class EditedAssetTransformer
    {
        public static void ToBargeHarbor()
        {
            var buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            buildingInfo.m_dlcRequired |= SteamHelper.DLC_BitMask.InMotionDLC;
            buildingInfo.m_isCustomContent = true;
            buildingInfo.m_class = ItemClasses.cargoFerryFacility;
            var cargoHarborAI = buildingInfo.m_buildingAI as CargoHarborAI;
            cargoHarborAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry");
        }

        public static void ToBarge() {
            var vehicleInfo = ToolsModifierControl.toolController.m_editPrefabInfo as VehicleInfo;
            vehicleInfo.m_dlcRequired |= SteamHelper.DLC_BitMask.InMotionDLC;
            vehicleInfo.m_vehicleType = VehicleInfo.VehicleType.Ferry;
            vehicleInfo.m_class = ItemClasses.cargoFerryVehicle;
            vehicleInfo.m_isCustomContent = true;
            var ai = ReplaceAI<CargoFerryAI>(vehicleInfo);
            ai.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry");
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
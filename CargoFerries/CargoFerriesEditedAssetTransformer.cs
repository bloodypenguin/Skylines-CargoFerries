using CargoFerries.AI;
using CargoFerries.Utils;
using UnityEngine;

namespace CargoFerries
{
    public static class CargoFerriesEditedAssetTransformer
    {
        public static void ToBargeHarbor()
        {
            var buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            if (buildingInfo?.m_buildingAI is not CargoHarborAI cargoHarborAI)
            {
                UnityEngine.Debug.LogWarning("Barges: Current asset is not a building or is not CargoHarborAI");
                return;
            }
            buildingInfo.m_dlcRequired |= SteamHelper.DLC_BitMask.InMotionDLC;
            buildingInfo.m_isCustomContent = true;
            buildingInfo.m_class = ItemClasses.cargoFerryFacility;
            cargoHarborAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry");
        }

        public static void ToBarge() {
            var vehicleInfo = ToolsModifierControl.toolController?.m_editPrefabInfo as VehicleInfo;
            if (vehicleInfo?.m_vehicleAI is not CargoShipAI cargoShipAI)
            {
                UnityEngine.Debug.LogWarning("Barges: Current asset is not a vehicle or is not CargoShipAI");
                return;
            }
            vehicleInfo.m_dlcRequired |= SteamHelper.DLC_BitMask.InMotionDLC;
            vehicleInfo.m_vehicleType = VehicleInfo.VehicleType.Ferry;
            vehicleInfo.m_class = ItemClasses.cargoFerryVehicle;
            vehicleInfo.m_isCustomContent = true;
            cargoShipAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry");
        }
    }
}
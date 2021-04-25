using ICities;

namespace CargoFerries
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            ItemClasses.Register();
            if (loading.currentMode != AppMode.Game)
            {
                return;
            }
            HarmonyPatches.BuildingInfoPatch. InitializePrefabPatch.Apply();
            HarmonyPatches.FerryAIPatch.SimulationStepPatch.Apply();
            HarmonyPatches.VehicleInfoPatch.InitializePrefabPatch.Apply();
            HarmonyPatches.CargoTruckAIPatch.NeedChangeVehicleTypePatch.Apply();
            HarmonyPatches.CargoTruckAIPatch.StartPathFindPatch.Apply();
            if (Util.IsModActive(1764208250))
            {
                UnityEngine.Debug.LogWarning("Barges: More Vehicles is enabled, applying compatibility workaround");
                CargoFerriesMod.MaxVehicleCount = ushort.MaxValue + 1;
            }
            else
            {
                UnityEngine.Debug.Log("Barges: More Vehicles is not enabled");
                CargoFerriesMod.MaxVehicleCount = VehicleManager.MAX_VEHICLE_COUNT;
            }
            if (Util.IsModActive("Service Vehicle Selector 2"))
            {
                UnityEngine.Debug.Log("Barges: Service Vehicle Selector 2 is detected! CargoTruckAI.ChangeVehicleType() won't be patched");
            } else {
                HarmonyPatches.CargoTruckAIPatch.ChangeVehicleTypePatch.Apply(); 
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            HarmonyPatches.BuildingInfoPatch.InitializePrefabPatch.Undo();
            HarmonyPatches.FerryAIPatch.SimulationStepPatch.Undo();
            HarmonyPatches.VehicleInfoPatch.InitializePrefabPatch.Undo();
            HarmonyPatches.CargoTruckAIPatch.NeedChangeVehicleTypePatch.Undo();
            HarmonyPatches.CargoTruckAIPatch.StartPathFindPatch.Undo();
            HarmonyPatches.CargoTruckAIPatch.ChangeVehicleTypePatch.Undo();
            ItemClasses.Unregister();
        }
    }
}
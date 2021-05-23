using CargoFerries.OptionsFramework;
using CitiesHarmony.API;
using ICities;
using UnityEngine;
using Util = CargoFerries.Utils.Util;

namespace CargoFerries
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static GameObject _gameObject;
        
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            ItemClasses.Register();
            if (loading.currentMode != AppMode.Game)
            {
                return;
            }
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }
            HarmonyPatches.BuildingInfoPatch. InitializePrefabPatch.Apply();
            HarmonyPatches.FerryAIPatch.SimulationStepPatch.Apply();
            HarmonyPatches.VehicleInfoPatch.InitializePrefabPatch.Apply();
            HarmonyPatches.CargoTruckAIPatch.NeedChangeVehicleTypePatch.Apply();
            HarmonyPatches.CargoTruckAIPatch.StartPathFindPatch.Apply();
            HarmonyPatches.PostVanAIPatch.StartPathFindPatch.Apply();
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

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (!OptionsWrapper<Options>.Options.EnableWarehouseAI)
            {
                return;
            }
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }
            if (_gameObject != null)
            {
                return;
            }
            _gameObject = new GameObject("CargoFerries");
            _gameObject.AddComponent<GamePanelExtender>();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (_gameObject == null)
            {
                return;
            }
            Object.Destroy(_gameObject);
            _gameObject = null;
        } 

        public override void OnReleased()
        {
            base.OnReleased();
            ItemClasses.Unregister();
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }
            HarmonyPatches.BuildingInfoPatch.InitializePrefabPatch.Undo();
            HarmonyPatches.FerryAIPatch.SimulationStepPatch.Undo();
            HarmonyPatches.VehicleInfoPatch.InitializePrefabPatch.Undo();
            HarmonyPatches.CargoTruckAIPatch.NeedChangeVehicleTypePatch.Undo();
            HarmonyPatches.CargoTruckAIPatch.StartPathFindPatch.Undo();
            HarmonyPatches.PostVanAIPatch.StartPathFindPatch.Undo();
            HarmonyPatches.CargoTruckAIPatch.ChangeVehicleTypePatch.Undo();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using CargoFerries.HarmonyPatches;
using ColossalFramework;
using ICities;
using ServiceVehicleSelector2.HarmonyPatches;

namespace CargoFerries
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            var dictionary = ((Dictionary<string, ItemClass>)typeof(ItemClassCollection).GetField("m_classDict", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
            if (!dictionary.ContainsKey(ItemClasses.cargoFerryFacility.name))
            {
                dictionary.Add(ItemClasses.cargoFerryFacility.name, ItemClasses.cargoFerryFacility);
            }
            if (!dictionary.ContainsKey(ItemClasses.cargoFerryVehicle.name))
            {
                dictionary.Add(ItemClasses.cargoFerryVehicle.name, ItemClasses.cargoFerryVehicle);
            }
            if (loading.currentMode != AppMode.Game)
            {
                return;
            }
            
            VehicleInfoPatch.Apply();
            CargoTruckVehicleTypePatch.Apply();
            BuildingInfoPatch.Apply();
        }

        private static void ReleaseWrongVehicles()
        {
            var toRelease = new List<ushort>();
            for (var i = 0; i < TransportManager.instance.m_lines.m_buffer.Length; i++)
            {
                var line = TransportManager.instance.m_lines.m_buffer[i];
                if (line.m_flags == TransportLine.Flags.None || line.Info == null)
                {
                    continue;
                }

                if (line.m_vehicles != 0)
                {
                    VehicleManager instance = VehicleManager.instance;
                    ushort num2 = line.m_vehicles;
                    int num3 = 0;
                    while (num2 != 0)
                    {
                        var vehicle = instance.m_vehicles.m_buffer[(int) num2];
                        long id;
                        if (Util.TryGetWorkshopId(vehicle.Info, out id))
                        {
                            if (line.Info.m_vehicleType != vehicle.Info.m_vehicleType)
                            {
                                line.RemoveVehicle(num2, ref instance.m_vehicles.m_buffer[(int) num2]);
                                toRelease.Add(num2);
                            }
                        }

                        ushort num4 = vehicle.m_nextLineVehicle;
                        num2 = num4;
                        if (++num3 > VehicleManager.MAX_VEHICLE_COUNT)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core,
                                "Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }

            foreach (var id in toRelease)
            {
                VehicleManager.instance.ReleaseVehicle(id);
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            SimulationManager.instance.AddAction(ReleaseWrongVehicles);
        }

        public override void OnReleased()
        {
            base.OnReleased();
            VehicleInfoPatch.Undo();
            CargoTruckVehicleTypePatch.Undo();
            BuildingInfoPatch.Undo();
            var dictionary = ((Dictionary<string, ItemClass>)typeof(ItemClassCollection).GetField("m_classDict", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
            dictionary.Remove(ItemClasses.cargoFerryFacility.name);
            dictionary.Remove(ItemClasses.cargoFerryVehicle.name);
        }
    }
}
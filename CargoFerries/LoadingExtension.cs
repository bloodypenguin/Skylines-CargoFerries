using System;
using System.Collections.Generic;
using System.Reflection;
using CargoFerries.HarmonyPatches;
using ColossalFramework;
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
            FerryAIPatch.Apply();
            VehicleInfoPatch.Apply();
            CargoTruckVehicleTypePatch.Apply();
            BuildingInfoPatch.Apply();
            if (Util.IsModActive("Service Vehicle Selector 2"))
            {
                UnityEngine.Debug.Log("MCM: Service Vehicle Selector 2 is detected! CargoTruckAI.ChangeVehicleType() won't be patched");
            } else {
                CargoTruckAIChangeVehicleTypePatch.Apply(); 
            }
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
            CargoTruckAIChangeVehicleTypePatch.Undo();
            FerryAIPatch.Undo();
            ItemClasses.Unregister();
        }
    }
}
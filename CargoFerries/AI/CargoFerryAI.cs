using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

namespace CargoFerries.AI
{
    public class CargoFerryAI : CargoShipAI
    {
        public override Matrix4x4 CalculateBodyMatrix(
            Vehicle.Flags flags,
            ref Vector3 position,
            ref Quaternion rotation,
            ref Vector3 scale,
            ref Vector3 swayPosition)
        {
            return FakeFerryAI.GetFakeShipAI(this)
                .CalculateBodyMatrix(flags, ref position, ref rotation, ref scale, ref swayPosition);
        }

        protected override void CalculateSegmentPosition(
            ushort vehicleID,
            ref Vehicle vehicleData,
            PathUnit.Position position,
            uint laneID,
            byte offset,
            out Vector3 pos,
            out Vector3 dir,
            out float maxSpeed)
        {
            FakeFerryAI.GetFakeShipAI(this)
                .CalculateSegmentPosition(vehicleID, ref vehicleData, position, laneID, offset, out pos, out dir,
                    out maxSpeed);
        }

        protected override void CalculateSegmentPosition(
            ushort vehicleID,
            ref Vehicle vehicleData,
            PathUnit.Position nextPosition,
            PathUnit.Position position,
            uint laneID,
            byte offset,
            PathUnit.Position prevPos,
            uint prevLaneID,
            byte prevOffset,
            int index,
            out Vector3 pos,
            out Vector3 dir,
            out float maxSpeed)
        {
            FakeFerryAI.GetFakeShipAI(this)
                .CalculateSegmentPosition(vehicleID, ref vehicleData, nextPosition, position, laneID, offset,
                    prevPos, prevLaneID, prevOffset, index,
                    out pos, out dir, out maxSpeed);
        }

        public override void SimulationStep(
            ushort vehicleID,
            ref Vehicle vehicleData,
            ref Vehicle.Frame frameData,
            ushort leaderID,
            ref Vehicle leaderData,
            int lodPhysics)
        {
            if ((VehicleManager.instance.m_vehicles.m_buffer[vehicleID].m_flags & Vehicle.Flags.Arriving) ==
                ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                  Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                  Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                  Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                  Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                  Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                  Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                  Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                  Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                  Vehicle.Flags.LeftHandDrive))
            {
                FakeFerryAI.GetFakeShipAI(this).SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID,
                    ref leaderData, lodPhysics);
            }
            else
            {
                base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);
            }
        }

        //TODO: copied from FerryAI + flag mod
        public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
        {
            if ((data.m_flags & Vehicle.Flags.WaitingPath) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                                Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                                Vehicle.Flags.TransferToTarget |
                                                                Vehicle.Flags.TransferToSource |
                                                                Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                                                Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                                                                Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                                                                Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                                                Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                                                                Vehicle.Flags.WaitingSpace |
                                                                Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
                                                                Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                                                                Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
                                                                Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                                                                Vehicle.Flags.WaitingLoading |
                                                                Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                                                                Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                                                Vehicle.Flags.InsideBuilding |
                                                                Vehicle.Flags.LeftHandDrive))
            {
                byte pathFindFlags = Singleton<PathManager>.instance.m_pathUnits.m_buffer[data.m_path].m_pathFindFlags;
                if (((int) pathFindFlags & 4) != 0)
                {
                    data.m_pathPositionIndex = byte.MaxValue;
                    data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                                    Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
                                    Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                                    Vehicle.Flags.Emergency2 | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                                    Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                    Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace |
                                    Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget |
                                    Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
                                    Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
                                    Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground |
                                    Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                                    Vehicle.Flags.LeftHandDrive;
                    data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                                    Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
                                    Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                                    Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                                    Vehicle.Flags.Leaving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                    Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace |
                                    Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget |
                                    Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
                                    Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
                                    Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground |
                                    Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                                    Vehicle.Flags.LeftHandDrive;
                    this.PathfindSuccess(vehicleID, ref data);
                    this.TrySpawn(vehicleID, ref data);
                }
                else if (((int) pathFindFlags & 8) != 0)
                {
                    data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                                    Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
                                    Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                                    Vehicle.Flags.Emergency2 | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                                    Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                    Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace |
                                    Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget |
                                    Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
                                    Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
                                    Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground |
                                    Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                                    Vehicle.Flags.LeftHandDrive;
                    Singleton<PathManager>.instance.ReleasePath(data.m_path);
                    data.m_path = 0U;
                    this.PathfindFailure(vehicleID, ref data);
                    return;
                }
            }
            else if ((data.m_flags & Vehicle.Flags.WaitingSpace) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                                      Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                                      Vehicle.Flags.TransferToTarget |
                                                                      Vehicle.Flags.TransferToSource |
                                                                      Vehicle.Flags.Emergency1 |
                                                                      Vehicle.Flags.Emergency2 |
                                                                      Vehicle.Flags.WaitingPath |
                                                                      Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                                                                      Vehicle.Flags.Arriving | Vehicle.Flags.Reversed |
                                                                      Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                                                                      Vehicle.Flags.Landing |
                                                                      Vehicle.Flags.WaitingSpace |
                                                                      Vehicle.Flags.WaitingCargo |
                                                                      Vehicle.Flags.GoingBack |
                                                                      Vehicle.Flags.WaitingTarget |
                                                                      Vehicle.Flags.Importing |
                                                                      Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
                                                                      Vehicle.Flags.CustomName |
                                                                      Vehicle.Flags.OnGravel |
                                                                      Vehicle.Flags.WaitingLoading |
                                                                      Vehicle.Flags.Congestion |
                                                                      Vehicle.Flags.DummyTraffic |
                                                                      Vehicle.Flags.Underground |
                                                                      Vehicle.Flags.Transition |
                                                                      Vehicle.Flags.InsideBuilding |
                                                                      Vehicle.Flags.LeftHandDrive))
                this.TrySpawn(vehicleID, ref data);

            Vector3 lastFramePosition = data.GetLastFramePosition();
            int lodPhysics = (double) Vector3.SqrMagnitude(physicsLodRefPos - lastFramePosition) < 1210000.0
                ? ((double) Vector3.SqrMagnitude(Singleton<SimulationManager>.instance.m_simulationView.m_position -
                                                 lastFramePosition) < 250000.0
                    ? 0
                    : 1)
                : 2;
            this.SimulationStep(vehicleID, ref data, vehicleID, ref data, lodPhysics);
            if (data.m_leadingVehicle == (ushort) 0 && data.m_trailingVehicle != (ushort) 0)
            {
                VehicleManager instance = Singleton<VehicleManager>.instance;
                ushort vehicleID1 = data.m_trailingVehicle;
                int num = 0;
                while (vehicleID1 != (ushort) 0)
                {
                    ushort trailingVehicle = instance.m_vehicles.m_buffer[(int) vehicleID1].m_trailingVehicle;
                    instance.m_vehicles.m_buffer[(int) vehicleID1].Info.m_vehicleAI.SimulationStep(vehicleID1,
                        ref instance.m_vehicles.m_buffer[(int) vehicleID1], vehicleID, ref data, lodPhysics);
                    vehicleID1 = trailingVehicle;
                    if (++num > 16384)
                    {
                        CODebugBase<LogChannel>.Error(LogChannel.Core,
                            "Invalid list detected!\n" + System.Environment.StackTrace);
                        break;
                    }
                }
            }

            int num1 = ItemClass.GetPrivateServiceIndex(this.m_info.m_class.m_service) == -1 ? 150 : 100;
            //added WaitingCargo
            if (
                (data.m_flags & (Vehicle.Flags.Spawned | Vehicle.Flags.WaitingPath | Vehicle.Flags.WaitingSpace |
                                 Vehicle.Flags.WaitingCargo)) ==
                ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                  Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                  Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                  Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                  Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                  Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                  Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                  Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                  Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                  Vehicle.Flags.LeftHandDrive) && data.m_cargoParent == (ushort) 0)
            {
                Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
            }
            else
            {
                if ((int) data.m_blockCounter != num1)
                    return;
                Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
            }
        }

        protected void PathfindSuccess(ushort vehicleID, ref Vehicle data)
        {
        }

        protected void PathfindFailure(ushort vehicleID, ref Vehicle data)
        {
            data.Unspawn(vehicleID);
        }


        protected override bool StartPathFind(
            ushort vehicleID,
            ref Vehicle vehicleData,
            Vector3 startPos,
            Vector3 endPos)
        {
            return FakeFerryAI.GetFakeShipAI(this).StartPathFind(vehicleID, ref vehicleData, startPos, endPos);
        }
        
        //TODO: InvalidPath?
    }
}
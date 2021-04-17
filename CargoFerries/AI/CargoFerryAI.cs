using ColossalFramework;
using ColossalFramework.Math;
using System;
using UnityEngine;

//based of FerryAI + parts from CargoShipAI
public class CargoFerryAI : VehicleAI
{
    [NonSerialized] private RenderGroup.MeshData m_underwaterMeshData;

    [CustomizableProperty("Cargo capacity")]
    public int m_cargoCapacity = 1;

    public TransportInfo m_transportInfo;

    public override Matrix4x4 CalculateBodyMatrix(
        Vehicle.Flags flags,
        ref Vector3 position,
        ref Quaternion rotation,
        ref Vector3 scale,
        ref Vector3 swayPosition)
    {
        Vector3 vector3_1 = rotation * new Vector3(0.0f, 0.0f, this.m_info.m_generatedInfo.m_size.z * 0.25f);
        Vector3 vector3_2 = rotation * new Vector3(this.m_info.m_generatedInfo.m_size.x * -0.5f, 0.0f, 0.0f);
        Vector3 worldPos1 = position + vector3_1 + vector3_2;
        Vector3 worldPos2 = position + vector3_1 - vector3_2;
        Vector3 worldPos3 = position - vector3_1 + vector3_2;
        Vector3 worldPos4 = position - vector3_1 - vector3_2;
        worldPos1.y = Singleton<TerrainManager>.instance.SampleBlockHeightSmoothWithWater(worldPos1, true, 0.0f);
        worldPos2.y = Singleton<TerrainManager>.instance.SampleBlockHeightSmoothWithWater(worldPos2, true, 0.0f);
        worldPos3.y = Singleton<TerrainManager>.instance.SampleBlockHeightSmoothWithWater(worldPos3, true, 0.0f);
        worldPos4.y = Singleton<TerrainManager>.instance.SampleBlockHeightSmoothWithWater(worldPos4, true, 0.0f);
        Vector3 vector3_3 = worldPos1 + worldPos2 - worldPos3 - worldPos4;
        Vector3 upwards = Vector3.Cross(worldPos1 + worldPos3 - worldPos2 - worldPos4, vector3_3);
        rotation = Quaternion.LookRotation(vector3_3, upwards);
        position.y =
            (float) (((double) worldPos1.y + (double) worldPos2.y + (double) worldPos3.y + (double) worldPos4.y) *
                     0.25);
        Quaternion quaternion = Quaternion.Euler(swayPosition.z * 57.29578f, 0.0f, swayPosition.x * -57.29578f);
        Matrix4x4 matrix4x4 = new Matrix4x4();
        matrix4x4.SetTRS(position, rotation * quaternion, scale);
        matrix4x4.m13 += swayPosition.y;
        return matrix4x4;
    }

    public override Matrix4x4 CalculateTyreMatrix(
        Vehicle.Flags flags,
        ref Vector3 position,
        ref Quaternion rotation,
        ref Vector3 scale,
        ref Matrix4x4 bodyMatrix)
    {
        return Matrix4x4.identity;
    }

    public override RenderGroup.MeshData GetEffectMeshData()
    {
        if (this.m_info.m_lodMeshData != null && this.m_underwaterMeshData == null)
        {
            Vector3[] vertices = this.m_info.m_lodMeshData.m_vertices;
            int[] triangles1 = this.m_info.m_lodMeshData.m_triangles;
            if (triangles1 != null)
            {
                int[] numArray = new int[vertices.Length];
                for (int index = 0; index < vertices.Length; ++index)
                    numArray[index] = -1;
                int length1 = 0;
                int length2 = 0;
                for (int index1 = 0; index1 < triangles1.Length; index1 += 3)
                {
                    int index2 = triangles1[index1];
                    int index3 = triangles1[index1 + 1];
                    int index4 = triangles1[index1 + 2];
                    Vector3 vector3_1 = vertices[index2];
                    Vector3 vector3_2 = vertices[index3];
                    Vector3 vector3_3 = vertices[index4];
                    if ((double) vector3_1.y < -2.0 || (double) vector3_2.y < -2.0 || (double) vector3_3.y < -2.0)
                    {
                        length2 += 3;
                        if (numArray[index2] == -1)
                            numArray[index2] = length1++;
                        if (numArray[index3] == -1)
                            numArray[index3] = length1++;
                        if (numArray[index4] == -1)
                            numArray[index4] = length1++;
                    }
                }

                this.m_underwaterMeshData = new RenderGroup.MeshData();
                this.m_underwaterMeshData.m_vertices = new Vector3[length1];
                this.m_underwaterMeshData.m_triangles = new int[length2];
                int num1 = 0;
                for (int index1 = 0; index1 < triangles1.Length; index1 += 3)
                {
                    int index2 = triangles1[index1];
                    int index3 = triangles1[index1 + 1];
                    int index4 = triangles1[index1 + 2];
                    Vector3 vector3_1 = vertices[index2];
                    Vector3 vector3_2 = vertices[index3];
                    Vector3 vector3_3 = vertices[index4];
                    if ((double) vector3_1.y < -2.0 || (double) vector3_2.y < -2.0 || (double) vector3_3.y < -2.0)
                    {
                        vector3_1.y = Mathf.Min(vector3_1.y, -2f);
                        vector3_1.z -= this.m_info.m_generatedInfo.m_size.z / 2f;
                        vector3_2.y = Mathf.Min(vector3_2.y, -2f);
                        vector3_2.z -= this.m_info.m_generatedInfo.m_size.z / 2f;
                        vector3_3.y = Mathf.Min(vector3_3.y, -2f);
                        vector3_3.z -= this.m_info.m_generatedInfo.m_size.z / 2f;
                        int index5 = numArray[index2];
                        int index6 = numArray[index3];
                        int index7 = numArray[index4];
                        int[] triangles2 = this.m_underwaterMeshData.m_triangles;
                        int index8 = num1;
                        int num2 = index8 + 1;
                        int num3 = index5;
                        triangles2[index8] = num3;
                        int[] triangles3 = this.m_underwaterMeshData.m_triangles;
                        int index9 = num2;
                        int num4 = index9 + 1;
                        int num5 = index6;
                        triangles3[index9] = num5;
                        int[] triangles4 = this.m_underwaterMeshData.m_triangles;
                        int index10 = num4;
                        num1 = index10 + 1;
                        int num6 = index7;
                        triangles4[index10] = num6;
                        this.m_underwaterMeshData.m_vertices[index5] = vector3_1;
                        this.m_underwaterMeshData.m_vertices[index6] = vector3_2;
                        this.m_underwaterMeshData.m_vertices[index7] = vector3_3;
                    }
                }

                this.m_underwaterMeshData.UpdateBounds();
            }
        }

        return this.m_underwaterMeshData;
    }

    public override void CreateVehicle(ushort vehicleID, ref Vehicle data)
    {
        base.CreateVehicle(vehicleID, ref data);
        bool hasWater;
        data.m_frame0.m_position.y =
            Singleton<TerrainManager>.instance.SampleBlockHeightSmoothWithWater(data.m_frame0.m_position, false, 0.0f,
                out hasWater);
        data.m_frame1.m_position.y = data.m_frame0.m_position.y;
        data.m_frame2.m_position.y = data.m_frame0.m_position.y;
        data.m_frame3.m_position.y = data.m_frame0.m_position.y;
        if (!hasWater)
            return;
        data.m_flags2 |= Vehicle.Flags2.Floating;
    }

    public override void ReleaseVehicle(ushort vehicleID, ref Vehicle data) => base.ReleaseVehicle(vehicleID, ref data);

    public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
    {
        if ((data.m_flags & Vehicle.Flags.WaitingPath) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                            Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                            Vehicle.Flags.TransferToTarget |
                                                            Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                                                            Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath |
                                                            Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                                                            Vehicle.Flags.Arriving | Vehicle.Flags.Reversed |
                                                            Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                                                            Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace |
                                                            Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
                                                            Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                                                            Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
                                                            Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                                                            Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                                                            Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground |
                                                            Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                                                            Vehicle.Flags.LeftHandDrive))
        {
            byte pathFindFlags = Singleton<PathManager>.instance.m_pathUnits.m_buffer[data.m_path].m_pathFindFlags;
            if (((int) pathFindFlags & 4) != 0)
            {
                data.m_pathPositionIndex = byte.MaxValue;
                data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                                Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
                                Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                                Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                                Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                                Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                                Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                                Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
                data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                                Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
                                Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                                Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                                Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                                Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                                Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                                Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
                this.PathfindSuccess(vehicleID, ref data);
                this.TrySpawn(vehicleID, ref data);
            }
            else if (((int) pathFindFlags & 8) != 0)
            {
                data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                                Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
                                Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                                Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                                Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                                Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                                Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                                Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
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
                                                                  Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                                                  Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                                                                  Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                                                                  Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                                                  Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                                                                  Vehicle.Flags.WaitingSpace |
                                                                  Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
                                                                  Vehicle.Flags.WaitingTarget |
                                                                  Vehicle.Flags.Importing | Vehicle.Flags.Exporting |
                                                                  Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                                                  Vehicle.Flags.OnGravel |
                                                                  Vehicle.Flags.WaitingLoading |
                                                                  Vehicle.Flags.Congestion |
                                                                  Vehicle.Flags.DummyTraffic |
                                                                  Vehicle.Flags.Underground | Vehicle.Flags.Transition |
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
        if ((data.m_flags & (Vehicle.Flags.Spawned | Vehicle.Flags.WaitingPath | Vehicle.Flags.WaitingSpace)) ==
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

    protected virtual void PathfindSuccess(ushort vehicleID, ref Vehicle data)
    {
    }

    protected virtual void PathfindFailure(ushort vehicleID, ref Vehicle data) => data.Unspawn(vehicleID);

    protected virtual float SlowDownThreshold(ref Vehicle vehicleData) => 0.1f;

    public override void SimulationStep(
        ushort vehicleID,
        ref Vehicle vehicleData,
        ref Vehicle.Frame frameData,
        ushort leaderID,
        ref Vehicle leaderData,
        int lodPhysics)
    {
        uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
        frameData.m_position += frameData.m_velocity * 0.5f;
        frameData.m_swayPosition += frameData.m_swayVelocity * 0.5f;
        float acceleration = this.m_info.m_acceleration;
        float braking = this.m_info.m_braking;
        float num1 = VectorUtils.LengthXZ(frameData.m_velocity);
        Vector3 v1 = (Vector3) vehicleData.m_targetPos0 - frameData.m_position;
        float f1 = VectorUtils.LengthSqrXZ(v1);
        float maxDistance =
            (float) (((double) num1 + (double) acceleration) *
                     (0.5 + 0.5 * ((double) num1 + (double) acceleration) / (double) braking) +
                     (double) this.m_info.m_generatedInfo.m_size.z * 0.5);
        float num2 = Mathf.Max(num1 + acceleration, 5f);
        if (lodPhysics >= 2 && (long) (currentFrameIndex >> 4 & 3U) == (long) ((int) vehicleID & 3))
            num2 *= 2f;
        float num3 = Mathf.Max((float) (((double) maxDistance - (double) num2) / 3.0), 1f);
        float minSqrDistanceA = num2 * num2;
        float minSqrDistanceB = num3 * num3;
        int index = 0;
        bool flag1 = false;
        if (((double) f1 < (double) minSqrDistanceA || (double) vehicleData.m_targetPos3.w < 0.00999999977648258) &&
            (leaderData.m_flags & (Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped)) == ~(Vehicle.Flags.Created |
                Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
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
            if (leaderData.m_path != 0U)
            {
                this.UpdatePathTargetPositions(vehicleID, ref vehicleData, frameData.m_position, ref index, 4,
                    minSqrDistanceA, minSqrDistanceB);
                if ((leaderData.m_flags & Vehicle.Flags.Spawned) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
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
                {
                    frameData = vehicleData.m_frame0;
                    return;
                }
            }

            if ((leaderData.m_flags & Vehicle.Flags.WaitingPath) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
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
            {
                while (index < 4)
                {
                    float minSqrDistance;
                    Vector3 refPos;
                    if (index == 0)
                    {
                        minSqrDistance = minSqrDistanceA;
                        refPos = frameData.m_position;
                        flag1 = true;
                    }
                    else
                    {
                        minSqrDistance = minSqrDistanceB;
                        refPos = (Vector3) vehicleData.GetTargetPos(index - 1);
                    }

                    int num4 = index;
                    this.UpdateBuildingTargetPositions(vehicleID, ref vehicleData, refPos, leaderID, ref leaderData,
                        ref index, minSqrDistance);
                    if (index == num4)
                        break;
                }

                if (index != 0)
                {
                    Vector4 targetPos = vehicleData.GetTargetPos(index - 1);
                    while (index < 4)
                        vehicleData.SetTargetPos(index++, targetPos);
                }
            }

            v1 = (Vector3) vehicleData.m_targetPos0 - frameData.m_position;
            f1 = VectorUtils.LengthSqrXZ(v1);
        }

        if (leaderData.m_path != 0U)
        {
            NetManager instance1 = Singleton<NetManager>.instance;
            byte num4 = leaderData.m_pathPositionIndex;
            byte lastPathOffset = leaderData.m_lastPathOffset;
            if (num4 == byte.MaxValue)
                num4 = (byte) 0;
            PathManager instance2 = Singleton<PathManager>.instance;
            PathUnit.Position position;
            if (instance2.m_pathUnits.m_buffer[leaderData.m_path].GetPosition((int) num4 >> 1, out position))
            {
                instance1.m_segments.m_buffer[(int) position.m_segment]
                    .AddTraffic(Mathf.RoundToInt(this.m_info.m_generatedInfo.m_size.z * 3f), this.GetNoiseLevel());
                if (((int) num4 & 1) == 0 || lastPathOffset == (byte) 0 ||
                    (leaderData.m_flags & Vehicle.Flags.WaitingPath) != ~(Vehicle.Flags.Created |
                                                                          Vehicle.Flags.Deleted |
                                                                          Vehicle.Flags.Spawned |
                                                                          Vehicle.Flags.Inverted |
                                                                          Vehicle.Flags.TransferToTarget |
                                                                          Vehicle.Flags.TransferToSource |
                                                                          Vehicle.Flags.Emergency1 |
                                                                          Vehicle.Flags.Emergency2 |
                                                                          Vehicle.Flags.WaitingPath |
                                                                          Vehicle.Flags.Stopped |
                                                                          Vehicle.Flags.Leaving |
                                                                          Vehicle.Flags.Arriving |
                                                                          Vehicle.Flags.Reversed |
                                                                          Vehicle.Flags.TakingOff |
                                                                          Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                                                                          Vehicle.Flags.WaitingSpace |
                                                                          Vehicle.Flags.WaitingCargo |
                                                                          Vehicle.Flags.GoingBack |
                                                                          Vehicle.Flags.WaitingTarget |
                                                                          Vehicle.Flags.Importing |
                                                                          Vehicle.Flags.Exporting |
                                                                          Vehicle.Flags.Parking |
                                                                          Vehicle.Flags.CustomName |
                                                                          Vehicle.Flags.OnGravel |
                                                                          Vehicle.Flags.WaitingLoading |
                                                                          Vehicle.Flags.Congestion |
                                                                          Vehicle.Flags.DummyTraffic |
                                                                          Vehicle.Flags.Underground |
                                                                          Vehicle.Flags.Transition |
                                                                          Vehicle.Flags.InsideBuilding |
                                                                          Vehicle.Flags.LeftHandDrive))
                {
                    uint laneId = PathManager.GetLaneID(position);
                    if (laneId != 0U)
                        instance1.m_lanes.m_buffer[laneId].ReserveSpace(this.m_info.m_generatedInfo.m_size.z);
                }
                else if (instance2.m_pathUnits.m_buffer[leaderData.m_path]
                    .GetNextPosition((int) num4 >> 1, out position))
                {
                    uint laneId = PathManager.GetLaneID(position);
                    if (laneId != 0U)
                        instance1.m_lanes.m_buffer[laneId].ReserveSpace(this.m_info.m_generatedInfo.m_size.z);
                }
            }
        }

        float maxSpeed = (leaderData.m_flags & Vehicle.Flags.Stopped) != ~(Vehicle.Flags.Created |
                                                                           Vehicle.Flags.Deleted |
                                                                           Vehicle.Flags.Spawned |
                                                                           Vehicle.Flags.Inverted |
                                                                           Vehicle.Flags.TransferToTarget |
                                                                           Vehicle.Flags.TransferToSource |
                                                                           Vehicle.Flags.Emergency1 |
                                                                           Vehicle.Flags.Emergency2 |
                                                                           Vehicle.Flags.WaitingPath |
                                                                           Vehicle.Flags.Stopped |
                                                                           Vehicle.Flags.Leaving |
                                                                           Vehicle.Flags.Arriving |
                                                                           Vehicle.Flags.Reversed |
                                                                           Vehicle.Flags.TakingOff |
                                                                           Vehicle.Flags.Flying |
                                                                           Vehicle.Flags.Landing |
                                                                           Vehicle.Flags.WaitingSpace |
                                                                           Vehicle.Flags.WaitingCargo |
                                                                           Vehicle.Flags.GoingBack |
                                                                           Vehicle.Flags.WaitingTarget |
                                                                           Vehicle.Flags.Importing |
                                                                           Vehicle.Flags.Exporting |
                                                                           Vehicle.Flags.Parking |
                                                                           Vehicle.Flags.CustomName |
                                                                           Vehicle.Flags.OnGravel |
                                                                           Vehicle.Flags.WaitingLoading |
                                                                           Vehicle.Flags.Congestion |
                                                                           Vehicle.Flags.DummyTraffic |
                                                                           Vehicle.Flags.Underground |
                                                                           Vehicle.Flags.Transition |
                                                                           Vehicle.Flags.InsideBuilding |
                                                                           Vehicle.Flags.LeftHandDrive) ||
                         (leaderData.m_flags2 & Vehicle.Flags2.Floating) == (Vehicle.Flags2) 0
            ? 0.0f
            : vehicleData.m_targetPos0.w;
        Quaternion quaternion = Quaternion.Inverse(frameData.m_rotation);
        Vector3 v2 = quaternion * v1;
        Vector3 vector3_1 = quaternion * frameData.m_velocity;
        Vector3 vector3_2 = Vector3.forward;
        Vector3 zero1 = Vector3.zero;
        Vector3 zero2 = Vector3.zero;
        float f2 = 0.0f;
        float num5 = 0.0f;
        bool blocked = false;
        float len1 = 0.0f;
        if ((double) f1 > 1.0)
        {
            vector3_2 = VectorUtils.NormalizeXZ(v2, out len1);
            if ((double) len1 > 1.0)
            {
                Vector3 v3 = v2;
                float num4 = Mathf.Max(num1, 2f);
                float num6 = num4 * num4;
                if ((double) f1 > (double) num6)
                    v3 *= num4 / Mathf.Sqrt(f1);
                float len2;
                vector3_2 = VectorUtils.NormalizeXZ(v3, out len2);
                len1 = Mathf.Min(len1, len2);
                float curve = (float) (1.57079637050629 * (1.0 - (double) vector3_2.z));
                if ((double) len1 > 1.0)
                    curve /= len1;
                float targetDistance1 = len1;
                float a1 = (double) vehicleData.m_targetPos0.w >= 0.100000001490116
                    ? Mathf.Min(
                        Mathf.Min(maxSpeed, this.CalculateTargetSpeed(vehicleID, ref vehicleData, 1000f, curve)),
                        CargoFerryAI.CalculateMaxSpeed(targetDistance1, vehicleData.m_targetPos1.w, braking * 0.9f))
                    : Mathf.Min(this.CalculateTargetSpeed(vehicleID, ref vehicleData, 1000f, curve),
                        CargoFerryAI.CalculateMaxSpeed(targetDistance1,
                            Mathf.Min(vehicleData.m_targetPos0.w, vehicleData.m_targetPos1.w), braking * 0.9f));
                float targetDistance2 = targetDistance1 +
                                        VectorUtils.LengthXZ((Vector3) (vehicleData.m_targetPos1 -
                                                                        vehicleData.m_targetPos0));
                float a2 = Mathf.Min(a1,
                    CargoFerryAI.CalculateMaxSpeed(targetDistance2, vehicleData.m_targetPos2.w, braking * 0.9f));
                float targetDistance3 = targetDistance2 +
                                        VectorUtils.LengthXZ((Vector3) (vehicleData.m_targetPos2 -
                                                                        vehicleData.m_targetPos1));
                float a3 = Mathf.Min(a2,
                    CargoFerryAI.CalculateMaxSpeed(targetDistance3, vehicleData.m_targetPos3.w, braking * 0.9f));
                float targetDistance4 = targetDistance3 +
                                        VectorUtils.LengthXZ((Vector3) (vehicleData.m_targetPos3 -
                                                                        vehicleData.m_targetPos2));
                if ((double) vehicleData.m_targetPos3.w < 0.00999999977648258)
                    targetDistance4 = Mathf.Max(0.0f, targetDistance4 - this.m_info.m_generatedInfo.m_size.z * 0.5f);
                maxSpeed = Mathf.Min(a3, CargoFerryAI.CalculateMaxSpeed(targetDistance4, 0.0f, braking * 0.9f));
                if (!CargoFerryAI.DisableCollisionCheck(leaderID, ref leaderData))
                    CargoFerryAI.CheckOtherVehicles(vehicleID, ref vehicleData, ref frameData, ref maxSpeed,
                        ref blocked, ref zero2, maxDistance, braking * 0.9f, lodPhysics);
                if ((double) maxSpeed < (double) num1)
                {
                    float num7 = Mathf.Max(acceleration, Mathf.Min(braking, num1));
                    f2 = Mathf.Max(maxSpeed, num1 - num7);
                }
                else
                {
                    float num7 = Mathf.Max(acceleration, Mathf.Min(braking, -num1));
                    f2 = Mathf.Min(maxSpeed, num1 + num7);
                }
            }
        }
        else if ((double) num1 < (double) this.SlowDownThreshold(ref vehicleData) && flag1 &&
                 this.ArriveAtDestination(leaderID, ref leaderData))
        {
            leaderData.Unspawn(leaderID);
            if ((int) leaderID != (int) vehicleID)
                return;
            frameData = leaderData.m_frame0;
            return;
        }

        if ((leaderData.m_flags & Vehicle.Flags.Stopped) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                              Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                              Vehicle.Flags.TransferToTarget |
                                                              Vehicle.Flags.TransferToSource |
                                                              Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                                              Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                                                              Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                                                              Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                                              Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                                                              Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                                                              Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget |
                                                              Vehicle.Flags.Importing | Vehicle.Flags.Exporting |
                                                              Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                                              Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
                                                              Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                                                              Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                                              Vehicle.Flags.InsideBuilding |
                                                              Vehicle.Flags.LeftHandDrive) &&
            (double) maxSpeed < 0.100000001490116)
            blocked = true;
        vehicleData.m_blockCounter =
            !blocked ? (byte) 0 : (byte) Mathf.Min((int) vehicleData.m_blockCounter + 1, (int) byte.MaxValue);
        Vector3 vector3_3;
        if ((double) len1 > 1.0)
        {
            num5 = Mathf.Asin(vector3_2.x) * Mathf.Sign(f2);
            vector3_3 = vector3_2 * f2;
        }
        else
        {
            f2 = 0.0f;
            Vector3 vector = v2 * 0.5f - vector3_1;
            vector.y = 0.0f;
            Vector3 vector3_4 = Vector3.ClampMagnitude(vector, braking);
            vector3_3 = vector3_1 + vector3_4;
        }

        bool flag2 = ((int) currentFrameIndex + (int) leaderID & 16) != 0;
        Vector3 vector3_5 = vector3_3 - vector3_1;
        vector3_5.y = 0.0f;
        Vector3 forward1 = frameData.m_rotation * vector3_3;
        bool hasWater;
        forward1.y =
            Singleton<TerrainManager>.instance.SampleBlockHeightSmoothWithWater(
                frameData.m_position + frameData.m_velocity, false, 0.0f, out hasWater) - frameData.m_position.y;
        if (hasWater)
            leaderData.m_flags2 |= Vehicle.Flags2.Floating;
        else
            leaderData.m_flags2 &= ~Vehicle.Flags2.Floating;
        frameData.m_velocity = forward1 + zero2;
        frameData.m_position += frameData.m_velocity * 0.5f;
        frameData.m_swayVelocity = frameData.m_swayVelocity * (1f - this.m_info.m_dampers) -
                                   vector3_5 * (1f - this.m_info.m_springs) -
                                   frameData.m_swayPosition * this.m_info.m_springs;
        frameData.m_swayPosition += frameData.m_swayVelocity * 0.5f;
        frameData.m_steerAngle = num5;
        frameData.m_travelDistance += vector3_3.z;
        frameData.m_lightIntensity.x = 5f;
        frameData.m_lightIntensity.y = (double) vector3_5.z >= -0.100000001490116 ? 0.5f : 5f;
        frameData.m_lightIntensity.z = (double) num5 >= -0.100000001490116 || !flag2 ? 0.0f : 5f;
        frameData.m_lightIntensity.w = (double) num5 <= 0.100000001490116 || !flag2 ? 0.0f : 5f;
        frameData.m_underground = (vehicleData.m_flags & Vehicle.Flags.Underground) != ~(Vehicle.Flags.Created |
            Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
            Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
            Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
            Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing |
            Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
            Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
            Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
            Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground |
            Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive);
        frameData.m_transition = (vehicleData.m_flags & Vehicle.Flags.Transition) != ~(Vehicle.Flags.Created |
            Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
            Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
            Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
            Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing |
            Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
            Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
            Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
            Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground |
            Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive);
        if ((vehicleData.m_flags & Vehicle.Flags.Parking) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                               Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                               Vehicle.Flags.TransferToTarget |
                                                               Vehicle.Flags.TransferToSource |
                                                               Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                                               Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                                                               Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                                                               Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                                               Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                                                               Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                                                               Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget |
                                                               Vehicle.Flags.Importing | Vehicle.Flags.Exporting |
                                                               Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                                               Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
                                                               Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                                                               Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                                               Vehicle.Flags.InsideBuilding |
                                                               Vehicle.Flags.LeftHandDrive) && (double) len1 <= 1.0 &&
            flag1)
        {
            Vector3 forward2 = (Vector3) (vehicleData.m_targetPos1 - vehicleData.m_targetPos0);
            forward2.y = 0.0f;
            if ((double) forward2.sqrMagnitude > 0.00999999977648258)
                frameData.m_rotation =
                    Quaternion.RotateTowards(frameData.m_rotation, Quaternion.LookRotation(forward2), 15f);
        }
        else if ((double) f2 > 0.100000001490116)
        {
            forward1.y = 0.0f;
            if ((double) forward1.sqrMagnitude > 0.00999999977648258)
                frameData.m_rotation =
                    Quaternion.RotateTowards(frameData.m_rotation, Quaternion.LookRotation(forward1), 15f);
        }
        else if ((double) f2 < -0.100000001490116)
        {
            forward1.y = 0.0f;
            if ((double) forward1.sqrMagnitude > 0.00999999977648258)
                frameData.m_rotation =
                    Quaternion.RotateTowards(frameData.m_rotation, Quaternion.LookRotation(-forward1), 15f);
        }

        base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);
    }

    private static bool DisableCollisionCheck(ushort vehicleID, ref Vehicle vehicleData) =>
        (vehicleData.m_flags & Vehicle.Flags.Arriving) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                            Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                            Vehicle.Flags.TransferToTarget |
                                                            Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                                                            Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath |
                                                            Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                                                            Vehicle.Flags.Arriving | Vehicle.Flags.Reversed |
                                                            Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                                                            Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace |
                                                            Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
                                                            Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                                                            Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
                                                            Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                                                            Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                                                            Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground |
                                                            Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                                                            Vehicle.Flags.LeftHandDrive) &&
        (double) Mathf.Max(Mathf.Abs(vehicleData.m_targetPos3.x), Mathf.Abs(vehicleData.m_targetPos3.z)) >
        8640.0 - 100.0;

    protected Vector4 CalculateTargetPoint(
        Vector3 refPos,
        Vector3 targetPos,
        float maxSqrDistance,
        float speed)
    {
        Vector3 vector3 = targetPos - refPos;
        float sqrMagnitude = vector3.sqrMagnitude;
        Vector4 vector4 = (double) sqrMagnitude <= (double) maxSqrDistance
            ? (Vector4) targetPos
            : (Vector4) (refPos + vector3 * Mathf.Sqrt(maxSqrDistance / sqrMagnitude));
        vector4.w = speed;
        return vector4;
    }

    public override void FrameDataUpdated(
        ushort vehicleID,
        ref Vehicle vehicleData,
        ref Vehicle.Frame frameData)
    {
        Vector3 vector3_1 = frameData.m_position + frameData.m_velocity * 0.5f;
        Vector3 vector3_2 = frameData.m_rotation * new Vector3(0.0f, 0.0f,
            Mathf.Max(0.5f, (float) ((double) this.m_info.m_generatedInfo.m_size.z * 0.5 - 1.0)));
        vehicleData.m_segment.a = vector3_1 - vector3_2;
        vehicleData.m_segment.b = vector3_1 + vector3_2;
    }

    public static void CheckOtherVehicles(
        ushort vehicleID,
        ref Vehicle vehicleData,
        ref Vehicle.Frame frameData,
        ref float maxSpeed,
        ref bool blocked,
        ref Vector3 collisionPush,
        float maxDistance,
        float maxBraking,
        int lodPhysics)
    {
        Vector3 vector = (Vector3) vehicleData.m_targetPos3 - frameData.m_position;
        Vector3 rhs = frameData.m_position + Vector3.ClampMagnitude(vector, maxDistance);
        Vector3 min = Vector3.Min(vehicleData.m_segment.Min(), rhs);
        Vector3 max = Vector3.Max(vehicleData.m_segment.Max(), rhs);
        VehicleManager instance = Singleton<VehicleManager>.instance;
        int num1 = Mathf.Max((int) (((double) min.x - 10.0) / 320.0 + 27.0), 0);
        int num2 = Mathf.Max((int) (((double) min.z - 10.0) / 320.0 + 27.0), 0);
        int num3 = Mathf.Min((int) (((double) max.x + 10.0) / 320.0 + 27.0), 53);
        int num4 = Mathf.Min((int) (((double) max.z + 10.0) / 320.0 + 27.0), 53);
        for (int index1 = num2; index1 <= num4; ++index1)
        {
            for (int index2 = num1; index2 <= num3; ++index2)
            {
                ushort otherID = instance.m_vehicleGrid2[index1 * 54 + index2];
                int num5 = 0;
                while (otherID != (ushort) 0)
                {
                    otherID = CargoFerryAI.CheckOtherVehicle(vehicleID, ref vehicleData, ref frameData, ref maxSpeed,
                        ref blocked, ref collisionPush, maxBraking, otherID,
                        ref instance.m_vehicles.m_buffer[(int) otherID], min, max, lodPhysics);
                    if (++num5 > 16384)
                    {
                        CODebugBase<LogChannel>.Error(LogChannel.Core,
                            "Invalid list detected!\n" + System.Environment.StackTrace);
                        break;
                    }
                }
            }
        }
    }

    private static ushort CheckOtherVehicle(
        ushort vehicleID,
        ref Vehicle vehicleData,
        ref Vehicle.Frame frameData,
        ref float maxSpeed,
        ref bool blocked,
        ref Vector3 collisionPush,
        float maxBraking,
        ushort otherID,
        ref Vehicle otherData,
        Vector3 min,
        Vector3 max,
        int lodPhysics)
    {
        if ((int) otherID != (int) vehicleID && (int) vehicleData.m_leadingVehicle != (int) otherID &&
            (int) vehicleData.m_trailingVehicle != (int) otherID)
        {
            VehicleInfo info = otherData.Info;
            Vector3 vector3_1;
            Vector3 vector3_2;
            if (lodPhysics >= 2)
            {
                vector3_1 = otherData.m_segment.Min();
                vector3_2 = otherData.m_segment.Max();
            }
            else
            {
                vector3_1 = Vector3.Min(otherData.m_segment.Min(), (Vector3) otherData.m_targetPos3);
                vector3_2 = Vector3.Max(otherData.m_segment.Max(), (Vector3) otherData.m_targetPos3);
            }

            if ((double) min.x < (double) vector3_2.x + 2.0 && (double) min.y < (double) vector3_2.y + 2.0 &&
                ((double) min.z < (double) vector3_2.z + 2.0 && (double) vector3_1.x < (double) max.x + 2.0) &&
                ((double) vector3_1.y < (double) max.y + 2.0 && (double) vector3_1.z < (double) max.z + 2.0))
            {
                Vehicle.Frame lastFrameData = otherData.GetLastFrameData();
                if (lodPhysics < 2)
                {
                    float num = vehicleData.m_segment.DistanceSqr(otherData.m_segment, out float _, out float _);
                    if ((double) num < 4.0)
                    {
                        Vector3 vector3_3 = vehicleData.m_segment.Position(0.5f);
                        Vector3 vector3_4 = otherData.m_segment.Position(0.5f);
                        Vector3 lhs = vehicleData.m_segment.b - vehicleData.m_segment.a;
                        if ((double) Vector3.Dot(lhs, vector3_3 - vector3_4) < 0.0)
                            collisionPush -= lhs.normalized *
                                             (float) (0.100000001490116 - (double) num * 0.025000000372529);
                        else
                            collisionPush += lhs.normalized *
                                             (float) (0.100000001490116 - (double) num * 0.025000000372529);
                        blocked = true;
                    }
                }

                float num1 = frameData.m_velocity.magnitude + 0.01f;
                float magnitude1 = lastFrameData.m_velocity.magnitude;
                float num2 = magnitude1 * (float) (0.5 + 0.5 * (double) magnitude1 / (double) info.m_braking) +
                             Mathf.Min(1f, magnitude1);
                float num3 = magnitude1 + 0.01f;
                float num4 = 0.0f;
                Vector3 _a1 = vehicleData.m_segment.b;
                Vector3 lhs1 = vehicleData.m_segment.b - vehicleData.m_segment.a;
                for (int index1 = 0; index1 < 4; ++index1)
                {
                    Vector3 targetPos = (Vector3) vehicleData.GetTargetPos(index1);
                    Vector3 vector3_3 = targetPos - _a1;
                    if ((double) Vector3.Dot(lhs1, vector3_3) > 0.0)
                    {
                        float magnitude2 = vector3_3.magnitude;
                        Segment3 segment1 = new Segment3(_a1, targetPos);
                        min = segment1.Min();
                        max = segment1.Max();
                        segment1.a.y *= 0.5f;
                        segment1.b.y *= 0.5f;
                        if ((double) magnitude2 > 0.00999999977648258 && (double) min.x < (double) vector3_2.x + 2.0 &&
                            ((double) min.y < (double) vector3_2.y + 2.0 &&
                             (double) min.z < (double) vector3_2.z + 2.0) &&
                            ((double) vector3_1.x < (double) max.x + 2.0 &&
                             (double) vector3_1.y < (double) max.y + 2.0 &&
                             (double) vector3_1.z < (double) max.z + 2.0))
                        {
                            Vector3 a = otherData.m_segment.a;
                            a.y *= 0.5f;
                            float u;
                            if ((double) segment1.DistanceSqr(a, out u) < 4.0)
                            {
                                float targetSpeed = Vector3.Dot(lastFrameData.m_velocity, vector3_3) / magnitude2;
                                float num5 = num4 + magnitude2 * u;
                                if ((double) num5 >= 0.00999999977648258)
                                {
                                    float b = Mathf.Max(0.0f,
                                        CargoFerryAI.CalculateMaxSpeed(num5 - (targetSpeed + 3f), targetSpeed,
                                            maxBraking));
                                    if ((double) b < 0.00999999977648258)
                                        blocked = true;
                                    Vector3 rhs =
                                        Vector3.Normalize((Vector3) otherData.m_targetPos0 - otherData.m_segment.a);
                                    float num6 = (float) (1.20000004768372 -
                                                          1.0 / ((double) vehicleData.m_blockCounter *
                                                              0.0199999995529652 + 0.5));
                                    if ((double) Vector3.Dot(vector3_3, rhs) > (double) num6 * (double) magnitude2)
                                    {
                                        maxSpeed = Mathf.Min(maxSpeed, b);
                                        break;
                                    }

                                    break;
                                }

                                break;
                            }

                            if (lodPhysics < 2)
                            {
                                float num5 = 0.0f;
                                float maxLength = num2;
                                Vector3 _a2 = otherData.m_segment.b;
                                Vector3 lhs2 = otherData.m_segment.b - otherData.m_segment.a;
                                int num6 = 0;
                                bool flag = false;
                                for (int index2 = num6; index2 < 4 && (double) maxLength > 0.100000001490116; ++index2)
                                {
                                    Vector3 vector3_4;
                                    if (otherData.m_leadingVehicle != (ushort) 0)
                                    {
                                        if (index2 == num6)
                                            vector3_4 = Singleton<VehicleManager>.instance.m_vehicles
                                                .m_buffer[(int) otherData.m_leadingVehicle].m_segment.b;
                                        else
                                            break;
                                    }
                                    else
                                        vector3_4 = (Vector3) otherData.GetTargetPos(index2);

                                    Vector3 rhs = Vector3.ClampMagnitude(vector3_4 - _a2, maxLength);
                                    if ((double) Vector3.Dot(lhs2, rhs) > 0.0)
                                    {
                                        Vector3 _b = _a2 + rhs;
                                        float magnitude3 = rhs.magnitude;
                                        maxLength -= magnitude3;
                                        Segment3 segment2 = new Segment3(_a2, _b);
                                        segment2.a.y *= 0.5f;
                                        segment2.b.y *= 0.5f;
                                        float num7;
                                        float num8;
                                        if ((double) magnitude3 > 0.00999999977648258 &&
                                            ((int) otherID >= (int) vehicleID
                                                ? (double) segment1.DistanceSqr(segment2, out num8, out num7)
                                                : (double) segment2.DistanceSqr(segment1, out num7, out num8)) < 4.0)
                                        {
                                            float num9 = num4 + magnitude2 * num8;
                                            float num10 = (float) ((double) num5 + (double) magnitude3 * (double) num7 +
                                                                   0.100000001490116);
                                            if ((double) num9 >= 0.00999999977648258 && (double) num9 * (double) num3 >
                                                (double) num10 * (double) num1)
                                            {
                                                float targetSpeed = Vector3.Dot(lastFrameData.m_velocity, vector3_3) /
                                                                    magnitude2;
                                                if ((double) num9 >= 0.00999999977648258)
                                                {
                                                    float b = Mathf.Max(0.0f,
                                                        CargoFerryAI.CalculateMaxSpeed(
                                                            num9 - (targetSpeed + 1f +
                                                                    otherData.Info.m_generatedInfo.m_size.z),
                                                            targetSpeed, maxBraking));
                                                    if ((double) b < 0.00999999977648258)
                                                        blocked = true;
                                                    maxSpeed = Mathf.Min(maxSpeed, b);
                                                }
                                            }

                                            flag = true;
                                            break;
                                        }

                                        lhs2 = rhs;
                                        num5 += magnitude3;
                                        _a2 = _b;
                                    }
                                }

                                if (flag)
                                    break;
                            }
                        }

                        lhs1 = vector3_3;
                        num4 += magnitude2;
                        _a1 = targetPos;
                    }
                }
            }
        }

        return otherData.m_nextGridVehicle;
    }

    protected new void UpdatePathTargetPositions(
        ushort vehicleID,
        ref Vehicle vehicleData,
        Vector3 refPos,
        ref int index,
        int max,
        float minSqrDistanceA,
        float minSqrDistanceB)
    {
        PathManager instance1 = Singleton<PathManager>.instance;
        NetManager instance2 = Singleton<NetManager>.instance;
        Vector4 vector4 = vehicleData.m_targetPos0;
        vector4.w = 1000f;
        float f = minSqrDistanceA;
        uint num1 = vehicleData.m_path;
        byte num2 = vehicleData.m_pathPositionIndex;
        byte offset1 = vehicleData.m_lastPathOffset;
        if (num2 == byte.MaxValue)
        {
            num2 = (byte) 0;
            if (index <= 0)
                vehicleData.m_pathPositionIndex = (byte) 0;
            if (!Singleton<PathManager>.instance.m_pathUnits.m_buffer[num1]
                .CalculatePathPositionOffset((int) num2 >> 1, (Vector3) vector4, out offset1))
            {
                this.InvalidPath(vehicleID, ref vehicleData, vehicleID, ref vehicleData);
                return;
            }
        }

        PathUnit.Position position1;
        if (!instance1.m_pathUnits.m_buffer[num1].GetPosition((int) num2 >> 1, out position1))
        {
            this.InvalidPath(vehicleID, ref vehicleData, vehicleID, ref vehicleData);
        }
        else
        {
            NetInfo info1 = instance2.m_segments.m_buffer[(int) position1.m_segment].Info;
            if (info1.m_lanes.Length <= (int) position1.m_lane)
            {
                this.InvalidPath(vehicleID, ref vehicleData, vehicleID, ref vehicleData);
            }
            else
            {
                uint num3 = PathManager.GetLaneID(position1);
                NetInfo.Lane lane1 = info1.m_lanes[(int) position1.m_lane];
                int index1;
                uint nextPath;
                PathUnit.Position position2;
                uint laneId;
                Bezier3 bezier3;
                while (true)
                {
                    if (((int) num2 & 1) == 0)
                    {
                        if (lane1.m_laneType != NetInfo.LaneType.CargoVehicle)
                        {
                            bool flag = true;
                            while ((int) offset1 != (int) position1.m_offset)
                            {
                                if (flag)
                                {
                                    flag = false;
                                }
                                else
                                {
                                    float num4 = Mathf.Sqrt(f) - VectorUtils.LengthXZ((Vector3) vector4 - refPos);
                                    int num5 = (double) num4 >= 0.0
                                        ? 4 + Mathf.Max(0,
                                            Mathf.CeilToInt((float) ((double) num4 * 256.0 /
                                                                     ((double) instance2.m_lanes.m_buffer[num3]
                                                                         .m_length + 1.0))))
                                        : 4;
                                    if ((int) offset1 > (int) position1.m_offset)
                                        offset1 = (byte) Mathf.Max((int) offset1 - num5, (int) position1.m_offset);
                                    else if ((int) offset1 < (int) position1.m_offset)
                                        offset1 = (byte) Mathf.Min((int) offset1 + num5, (int) position1.m_offset);
                                }

                                Vector3 pos1;
                                float maxSpeed;
                                this.CalculateSegmentPosition(vehicleID, ref vehicleData, position1, num3, offset1,
                                    out pos1, out Vector3 _, out maxSpeed);
                                vector4.Set(pos1.x, pos1.y, pos1.z, Mathf.Min(vector4.w, maxSpeed));
                                if ((double) VectorUtils.LengthSqrXZ(pos1 - refPos) >= (double) f)
                                {
                                    if (index <= 0)
                                        vehicleData.m_lastPathOffset = offset1;
                                    ref Vehicle local = ref vehicleData;
                                    int num4;
                                    index = (num4 = index) + 1;
                                    int index2 = num4;
                                    Vector4 pos2 = vector4;
                                    local.SetTargetPos(index2, pos2);
                                    f = minSqrDistanceB;
                                    refPos = (Vector3) vector4;
                                    vector4.w = 1000f;
                                    if (index == max)
                                        return;
                                }
                            }
                        }

                        ++num2;
                        offset1 = (byte) 0;
                        if (index <= 0)
                        {
                            vehicleData.m_pathPositionIndex = num2;
                            vehicleData.m_lastPathOffset = offset1;
                        }
                    }

                    index1 = ((int) num2 >> 1) + 1;
                    nextPath = num1;
                    if (index1 >= (int) instance1.m_pathUnits.m_buffer[num1].m_positionCount)
                    {
                        index1 = 0;
                        nextPath = instance1.m_pathUnits.m_buffer[num1].m_nextPathUnit;
                        if (nextPath == 0U)
                            break;
                    }

                    if (instance1.m_pathUnits.m_buffer[nextPath].GetPosition(index1, out position2))
                    {
                        NetInfo info2 = instance2.m_segments.m_buffer[(int) position2.m_segment].Info;
                        if (info2.m_lanes.Length > (int) position2.m_lane)
                        {
                            laneId = PathManager.GetLaneID(position2);
                            NetInfo.Lane lane2 = info2.m_lanes[(int) position2.m_lane];
                            ushort startNode1 = instance2.m_segments.m_buffer[(int) position1.m_segment].m_startNode;
                            ushort endNode1 = instance2.m_segments.m_buffer[(int) position1.m_segment].m_endNode;
                            ushort startNode2 = instance2.m_segments.m_buffer[(int) position2.m_segment].m_startNode;
                            ushort endNode2 = instance2.m_segments.m_buffer[(int) position2.m_segment].m_endNode;
                            if ((int) startNode2 == (int) startNode1 || (int) startNode2 == (int) endNode1 ||
                                ((int) endNode2 == (int) startNode1 || (int) endNode2 == (int) endNode1) ||
                                (((instance2.m_nodes.m_buffer[(int) startNode1].m_flags |
                                   instance2.m_nodes.m_buffer[(int) endNode1].m_flags) & NetNode.Flags.Disabled) !=
                                 NetNode.Flags.None ||
                                 ((instance2.m_nodes.m_buffer[(int) startNode2].m_flags |
                                   instance2.m_nodes.m_buffer[(int) endNode2].m_flags) & NetNode.Flags.Disabled) ==
                                 NetNode.Flags.None))
                            {
                                if (lane2.m_laneType != NetInfo.LaneType.Pedestrian)
                                {
                                    if ((lane2.m_laneType & (NetInfo.LaneType.Vehicle | NetInfo.LaneType.CargoVehicle |
                                                             NetInfo.LaneType.TransportVehicle)) !=
                                        NetInfo.LaneType.None)
                                    {
                                        if (lane2.m_vehicleType == this.m_info.m_vehicleType ||
                                            !this.NeedChangeVehicleType(vehicleID, ref vehicleData, position2, laneId,
                                                lane2.m_vehicleType, ref vector4))
                                        {
                                            if ((int) position2.m_segment != (int) position1.m_segment &&
                                                vehicleID != (ushort) 0)
                                                vehicleData.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                                       Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                                       Vehicle.Flags.TransferToTarget |
                                                                       Vehicle.Flags.TransferToSource |
                                                                       Vehicle.Flags.Emergency1 |
                                                                       Vehicle.Flags.Emergency2 |
                                                                       Vehicle.Flags.WaitingPath |
                                                                       Vehicle.Flags.Stopped | Vehicle.Flags.Arriving |
                                                                       Vehicle.Flags.Reversed |
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
                                                                       Vehicle.Flags.LeftHandDrive;
                                            byte offset2 = 0;
                                            if ((vehicleData.m_flags & Vehicle.Flags.Flying) !=
                                                ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                  Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                  Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource |
                                                  Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                                  Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                                                  Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                                                  Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                                  Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                                                  Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                                                  Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget |
                                                  Vehicle.Flags.Importing | Vehicle.Flags.Exporting |
                                                  Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                                  Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
                                                  Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                                                  Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                                  Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
                                                offset2 = position2.m_offset < (byte) 128 ? byte.MaxValue : (byte) 0;
                                            else if ((int) num3 != (int) laneId &&
                                                     lane1.m_laneType != NetInfo.LaneType.CargoVehicle)
                                            {
                                                PathUnit.CalculatePathPositionOffset(laneId, (Vector3) vector4,
                                                    out offset2);
                                                bezier3 = new Bezier3();
                                                Vector3 dir1;
                                                this.CalculateSegmentPosition(vehicleID, ref vehicleData, position1,
                                                    num3, position1.m_offset, out bezier3.a, out dir1, out float _);
                                                bool flag = offset1 == (byte) 0;
                                                if (flag)
                                                    flag = (vehicleData.m_flags & Vehicle.Flags.Reversed) ==
                                                           ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                             Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                             Vehicle.Flags.TransferToTarget |
                                                             Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                                                             Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath |
                                                             Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                                                             Vehicle.Flags.Arriving | Vehicle.Flags.Reversed |
                                                             Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                                                             Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace |
                                                             Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
                                                             Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                                                             Vehicle.Flags.Exporting | Vehicle.Flags.Parking |
                                                             Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                                                             Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                                                             Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground |
                                                             Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                                                             Vehicle.Flags.LeftHandDrive)
                                                        ? vehicleData.m_leadingVehicle == (ushort) 0
                                                        : vehicleData.m_trailingVehicle == (ushort) 0;
                                                Vector3 dir2;
                                                float maxSpeed;
                                                if (flag)
                                                {
                                                    PathUnit.Position position3;
                                                    if (!instance1.m_pathUnits.m_buffer[nextPath]
                                                        .GetNextPosition(index1, out position3))
                                                        position3 = new PathUnit.Position();
                                                    this.CalculateSegmentPosition(vehicleID, ref vehicleData, position3,
                                                        position2, laneId, offset2, position1, num3, position1.m_offset,
                                                        index, out bezier3.d, out dir2, out maxSpeed);
                                                }
                                                else
                                                    this.CalculateSegmentPosition(vehicleID, ref vehicleData, position2,
                                                        laneId, offset2, out bezier3.d, out dir2, out maxSpeed);

                                                if ((double) maxSpeed >= 0.00999999977648258 &&
                                                    (instance2.m_segments.m_buffer[(int) position2.m_segment].m_flags &
                                                     (NetSegment.Flags.Collapsed | NetSegment.Flags.Flooded)) ==
                                                    NetSegment.Flags.None)
                                                {
                                                    if (position1.m_offset == (byte) 0)
                                                        dir1 = -dir1;
                                                    if ((int) offset2 < (int) position2.m_offset)
                                                        dir2 = -dir2;
                                                    dir1.Normalize();
                                                    dir2.Normalize();
                                                    float distance;
                                                    NetSegment.CalculateMiddlePoints(bezier3.a, dir1, bezier3.d, dir2,
                                                        true, true, out bezier3.b, out bezier3.c, out distance);
                                                    if ((double) distance > 1.0)
                                                    {
                                                        ushort nodeID;
                                                        switch (offset2)
                                                        {
                                                            case 0:
                                                                nodeID = instance2.m_segments
                                                                    .m_buffer[(int) position2.m_segment].m_startNode;
                                                                break;
                                                            case byte.MaxValue:
                                                                nodeID = instance2.m_segments
                                                                    .m_buffer[(int) position2.m_segment].m_endNode;
                                                                break;
                                                            default:
                                                                nodeID = (ushort) 0;
                                                                break;
                                                        }

                                                        float curve = (float) (1.57079637050629 *
                                                                               (1.0 + (double) Vector3.Dot(dir1,
                                                                                   dir2)));
                                                        if ((double) distance > 1.0)
                                                            curve /= distance;
                                                        maxSpeed = Mathf.Min(maxSpeed,
                                                            this.CalculateTargetSpeed(vehicleID, ref vehicleData, 1000f,
                                                                curve));
                                                        while (offset1 < byte.MaxValue)
                                                        {
                                                            float num4 = Mathf.Sqrt(f) -
                                                                         Vector3.Distance((Vector3) vector4, refPos);
                                                            int num5 = (double) num4 >= 0.0
                                                                ? 8 + Mathf.Max(0,
                                                                    Mathf.CeilToInt((float) ((double) num4 * 256.0 /
                                                                        ((double) distance + 1.0))))
                                                                : 8;
                                                            offset1 = (byte) Mathf.Min((int) offset1 + num5,
                                                                (int) byte.MaxValue);
                                                            Vector3 vector3 =
                                                                bezier3.Position((float) offset1 * 0.003921569f);
                                                            vector4.Set(vector3.x, vector3.y, vector3.z,
                                                                Mathf.Min(vector4.w, maxSpeed));
                                                            if ((double) VectorUtils.LengthSqrXZ(vector3 - refPos) >=
                                                                (double) f)
                                                            {
                                                                if (index <= 0)
                                                                    vehicleData.m_lastPathOffset = offset1;
                                                                if (nodeID != (ushort) 0)
                                                                    this.UpdateNodeTargetPos(vehicleID, ref vehicleData,
                                                                        nodeID,
                                                                        ref instance2.m_nodes.m_buffer[(int) nodeID],
                                                                        ref vector4, index);
                                                                ref Vehicle local = ref vehicleData;
                                                                int num6;
                                                                index = (num6 = index) + 1;
                                                                int index2 = num6;
                                                                Vector4 pos = vector4;
                                                                local.SetTargetPos(index2, pos);
                                                                f = minSqrDistanceB;
                                                                refPos = (Vector3) vector4;
                                                                vector4.w = 1000f;
                                                                if (index == max)
                                                                    return;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                    goto label_76;
                                            }
                                            else
                                                PathUnit.CalculatePathPositionOffset(laneId, (Vector3) vector4,
                                                    out offset2);

                                            if (index <= 0)
                                            {
                                                if (index1 == 0)
                                                    Singleton<PathManager>.instance.ReleaseFirstUnit(
                                                        ref vehicleData.m_path);
                                                if (index1 >= (int) instance1.m_pathUnits.m_buffer[nextPath]
                                                        .m_positionCount - 1 &&
                                                    instance1.m_pathUnits.m_buffer[nextPath].m_nextPathUnit == 0U &&
                                                    vehicleID != (ushort) 0)
                                                    this.ArrivingToDestination(vehicleID, ref vehicleData);
                                            }

                                            num1 = nextPath;
                                            num2 = (byte) (index1 << 1);
                                            offset1 = offset2;
                                            if (index <= 0)
                                            {
                                                vehicleData.m_pathPositionIndex = num2;
                                                vehicleData.m_lastPathOffset = offset1;
                                                vehicleData.m_flags = vehicleData.m_flags & (Vehicle.Flags.Created |
                                                                          Vehicle.Flags.Deleted |
                                                                          Vehicle.Flags.Spawned |
                                                                          Vehicle.Flags.Inverted |
                                                                          Vehicle.Flags.TransferToTarget |
                                                                          Vehicle.Flags.TransferToSource |
                                                                          Vehicle.Flags.Emergency1 |
                                                                          Vehicle.Flags.Emergency2 |
                                                                          Vehicle.Flags.WaitingPath |
                                                                          Vehicle.Flags.Stopped |
                                                                          Vehicle.Flags.Leaving |
                                                                          Vehicle.Flags.Arriving |
                                                                          Vehicle.Flags.Reversed |
                                                                          Vehicle.Flags.TakingOff |
                                                                          Vehicle.Flags.Flying |
                                                                          Vehicle.Flags.Landing |
                                                                          Vehicle.Flags.WaitingSpace |
                                                                          Vehicle.Flags.WaitingCargo |
                                                                          Vehicle.Flags.GoingBack |
                                                                          Vehicle.Flags.WaitingTarget |
                                                                          Vehicle.Flags.Importing |
                                                                          Vehicle.Flags.Exporting |
                                                                          Vehicle.Flags.Parking |
                                                                          Vehicle.Flags.CustomName |
                                                                          Vehicle.Flags.WaitingLoading |
                                                                          Vehicle.Flags.Congestion |
                                                                          Vehicle.Flags.DummyTraffic |
                                                                          Vehicle.Flags.InsideBuilding |
                                                                          Vehicle.Flags.LeftHandDrive) |
                                                                      info2.m_setVehicleFlags;
                                                if (this.LeftHandDrive(lane2))
                                                    vehicleData.m_flags |= Vehicle.Flags.LeftHandDrive;
                                                else
                                                    vehicleData.m_flags &= Vehicle.Flags.Created |
                                                                           Vehicle.Flags.Deleted |
                                                                           Vehicle.Flags.Spawned |
                                                                           Vehicle.Flags.Inverted |
                                                                           Vehicle.Flags.TransferToTarget |
                                                                           Vehicle.Flags.TransferToSource |
                                                                           Vehicle.Flags.Emergency1 |
                                                                           Vehicle.Flags.Emergency2 |
                                                                           Vehicle.Flags.WaitingPath |
                                                                           Vehicle.Flags.Stopped |
                                                                           Vehicle.Flags.Leaving |
                                                                           Vehicle.Flags.Arriving |
                                                                           Vehicle.Flags.Reversed |
                                                                           Vehicle.Flags.TakingOff |
                                                                           Vehicle.Flags.Flying |
                                                                           Vehicle.Flags.Landing |
                                                                           Vehicle.Flags.WaitingSpace |
                                                                           Vehicle.Flags.WaitingCargo |
                                                                           Vehicle.Flags.GoingBack |
                                                                           Vehicle.Flags.WaitingTarget |
                                                                           Vehicle.Flags.Importing |
                                                                           Vehicle.Flags.Exporting |
                                                                           Vehicle.Flags.Parking |
                                                                           Vehicle.Flags.CustomName |
                                                                           Vehicle.Flags.OnGravel |
                                                                           Vehicle.Flags.WaitingLoading |
                                                                           Vehicle.Flags.Congestion |
                                                                           Vehicle.Flags.DummyTraffic |
                                                                           Vehicle.Flags.Underground |
                                                                           Vehicle.Flags.Transition |
                                                                           Vehicle.Flags.InsideBuilding;
                                            }

                                            position1 = position2;
                                            num3 = laneId;
                                            lane1 = lane2;
                                        }
                                        else
                                            goto label_51;
                                    }
                                    else
                                        goto label_49;
                                }
                                else
                                    goto label_39;
                            }
                            else
                                goto label_37;
                        }
                        else
                            goto label_35;
                    }
                    else
                        goto label_33;
                }

                if (index <= 0)
                {
                    Singleton<PathManager>.instance.ReleasePath(vehicleData.m_path);
                    vehicleData.m_path = 0U;
                }

                vector4.w = 1f;
                ref Vehicle local1 = ref vehicleData;
                int num7;
                index = (num7 = index) + 1;
                int index3 = num7;
                Vector4 pos3 = vector4;
                local1.SetTargetPos(index3, pos3);
                return;
                label_33:
                this.InvalidPath(vehicleID, ref vehicleData, vehicleID, ref vehicleData);
                return;
                label_35:
                this.InvalidPath(vehicleID, ref vehicleData, vehicleID, ref vehicleData);
                return;
                label_37:
                this.InvalidPath(vehicleID, ref vehicleData, vehicleID, ref vehicleData);
                return;
                label_39:
                if (vehicleID == (ushort) 0 || (vehicleData.m_flags & Vehicle.Flags.Parking) !=
                    ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                      Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                      Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                      Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed |
                      Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                      Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
                      Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting |
                      Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                      Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                      Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                      Vehicle.Flags.LeftHandDrive))
                    return;
                byte offset3 = position1.m_offset;
                byte segmentOffset = position1.m_offset;
                if (this.ParkVehicle(vehicleID, ref vehicleData, position1, nextPath, index1 << 1, out segmentOffset))
                {
                    if ((int) segmentOffset != (int) offset3)
                    {
                        if (index <= 0)
                        {
                            vehicleData.m_pathPositionIndex &= (byte) 254;
                            vehicleData.m_lastPathOffset = offset3;
                        }

                        position1.m_offset = segmentOffset;
                        instance1.m_pathUnits.m_buffer[num1].SetPosition((int) num2 >> 1, position1);
                    }

                    vehicleData.m_flags |= Vehicle.Flags.Parking;
                    return;
                }

                this.InvalidPath(vehicleID, ref vehicleData, vehicleID, ref vehicleData);
                return;
                label_49:
                this.InvalidPath(vehicleID, ref vehicleData, vehicleID, ref vehicleData);
                return;
                label_51:
                if ((double) VectorUtils.LengthSqrXZ((Vector3) vector4 - refPos) >= (double) f)
                {
                    ref Vehicle local2 = ref vehicleData;
                    int num4;
                    index = (num4 = index) + 1;
                    int index2 = num4;
                    Vector4 pos1 = vector4;
                    local2.SetTargetPos(index2, pos1);
                }

                if (index <= 0)
                {
                    while (index < max)
                    {
                        ref Vehicle local2 = ref vehicleData;
                        int num4;
                        index = (num4 = index) + 1;
                        int index2 = num4;
                        Vector4 pos1 = vector4;
                        local2.SetTargetPos(index2, pos1);
                    }

                    if ((int) nextPath != (int) vehicleData.m_path)
                        Singleton<PathManager>.instance.ReleaseFirstUnit(ref vehicleData.m_path);
                    vehicleData.m_pathPositionIndex = (byte) (index1 << 1);
                    PathUnit.CalculatePathPositionOffset(laneId, (Vector3) vector4, out vehicleData.m_lastPathOffset);
                    if (vehicleID == (ushort) 0 ||
                        this.ChangeVehicleType(vehicleID, ref vehicleData, position2, laneId))
                        return;
                    this.InvalidPath(vehicleID, ref vehicleData, vehicleID, ref vehicleData);
                    return;
                }

                while (index < max)
                {
                    ref Vehicle local2 = ref vehicleData;
                    int num4;
                    index = (num4 = index) + 1;
                    int index2 = num4;
                    Vector4 pos1 = vector4;
                    local2.SetTargetPos(index2, pos1);
                }

                return;
                label_76:
                if (index <= 0)
                    vehicleData.m_lastPathOffset = offset1;
                vector4 = (Vector4) bezier3.a;
                vector4.w = 0.0f;
                while (index < max)
                {
                    ref Vehicle local2 = ref vehicleData;
                    int num4;
                    index = (num4 = index) + 1;
                    int index2 = num4;
                    Vector4 pos1 = vector4;
                    local2.SetTargetPos(index2, pos1);
                }
            }
        }
    }

    private static bool CheckOverlap(Segment3 segment, ushort ignoreVehicle, float maxVelocity)
    {
        VehicleManager instance = Singleton<VehicleManager>.instance;
        Vector3 vector3_1 = segment.Min();
        Vector3 vector3_2 = segment.Max();
        int num1 = Mathf.Max((int) (((double) vector3_1.x - 10.0) / 320.0 + 27.0), 0);
        int num2 = Mathf.Max((int) (((double) vector3_1.z - 10.0) / 320.0 + 27.0), 0);
        int num3 = Mathf.Min((int) (((double) vector3_2.x + 10.0) / 320.0 + 27.0), 53);
        int num4 = Mathf.Min((int) (((double) vector3_2.z + 10.0) / 320.0 + 27.0), 53);
        bool overlap = false;
        for (int index1 = num2; index1 <= num4; ++index1)
        {
            for (int index2 = num1; index2 <= num3; ++index2)
            {
                ushort otherID = instance.m_vehicleGrid2[index1 * 54 + index2];
                int num5 = 0;
                while (otherID != (ushort) 0)
                {
                    otherID = CargoFerryAI.CheckOverlap(segment, ignoreVehicle, maxVelocity, otherID,
                        ref instance.m_vehicles.m_buffer[(int) otherID], ref overlap);
                    if (++num5 > 16384)
                    {
                        CODebugBase<LogChannel>.Error(LogChannel.Core,
                            "Invalid list detected!\n" + System.Environment.StackTrace);
                        break;
                    }
                }
            }
        }

        return overlap;
    }

    private static ushort CheckOverlap(
        Segment3 segment,
        ushort ignoreVehicle,
        float maxVelocity,
        ushort otherID,
        ref Vehicle otherData,
        ref bool overlap)
    {
        if ((ignoreVehicle == (ushort) 0 || (int) otherID != (int) ignoreVehicle &&
                (int) otherData.m_leadingVehicle != (int) ignoreVehicle &&
                (int) otherData.m_trailingVehicle != (int) ignoreVehicle) &&
            (double) segment.DistanceSqr(otherData.m_segment, out float _, out float _) < 4.0 &&
            (double) otherData.GetLastFrameData().m_velocity.sqrMagnitude < (double) maxVelocity * (double) maxVelocity)
            overlap = true;
        return otherData.m_nextGridVehicle;
    }

    private static float CalculateMaxSpeed(float targetDistance, float targetSpeed, float maxBraking)
    {
        float num1 = 0.5f * maxBraking;
        float num2 = num1 + targetSpeed;
        return Mathf.Sqrt(Mathf.Max(0.0f,
            (float) ((double) num2 * (double) num2 + 2.0 * (double) targetDistance * (double) maxBraking))) - num1;
    }

    protected override void InvalidPath(
        ushort vehicleID,
        ref Vehicle vehicleData,
        ushort leaderID,
        ref Vehicle leaderData)
    {
        vehicleData.m_targetPos0 = vehicleData.m_targetPos3;
        vehicleData.m_targetPos1 = vehicleData.m_targetPos3;
        vehicleData.m_targetPos2 = vehicleData.m_targetPos3;
        vehicleData.m_targetPos3.w = 0.0f;
        base.InvalidPath(vehicleID, ref vehicleData, leaderID, ref leaderData);
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
        NetManager instance = Singleton<NetManager>.instance;
        instance.m_lanes.m_buffer[laneID]
            .CalculatePositionAndDirection((float) offset * 0.003921569f, out pos, out dir);
        Vehicle.Frame lastFrameData = vehicleData.GetLastFrameData();
        Vector3 position1 = lastFrameData.m_position;
        Vector3 position2 = instance.m_lanes.m_buffer[prevLaneID].CalculatePosition((float) prevOffset * 0.003921569f);
        float num = (float) (0.5 * (double) lastFrameData.m_velocity.sqrMagnitude / (double) this.m_info.m_braking +
                             (double) this.m_info.m_generatedInfo.m_size.z * 0.5);
        if ((double) VectorUtils.LengthXZ(position1 - position2) >= (double) num - 5.0 &&
            !instance.m_lanes.m_buffer[laneID].CheckSpace(1000f, vehicleID))
        {
            ushort startNode = instance.m_segments.m_buffer[(int) prevPos.m_segment].m_startNode;
            ushort endNode = instance.m_segments.m_buffer[(int) prevPos.m_segment].m_endNode;
            uint lane1 = instance.m_nodes.m_buffer[(int) startNode].m_lane;
            uint lane2 = instance.m_nodes.m_buffer[(int) endNode].m_lane;
            if ((int) lane1 != (int) laneID || (int) lane2 != (int) laneID)
            {
                maxSpeed = 0.0f;
                return;
            }
        }

        NetInfo info = instance.m_segments.m_buffer[(int) position.m_segment].Info;
        if (info.m_lanes != null && info.m_lanes.Length > (int) position.m_lane)
            maxSpeed = this.CalculateTargetSpeed(vehicleID, ref vehicleData,
                info.m_lanes[(int) position.m_lane].m_speedLimit, instance.m_lanes.m_buffer[laneID].m_curve);
        else
            maxSpeed = this.CalculateTargetSpeed(vehicleID, ref vehicleData, 1f, 0.0f);
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
        NetManager instance = Singleton<NetManager>.instance;
        instance.m_lanes.m_buffer[laneID]
            .CalculatePositionAndDirection((float) offset * 0.003921569f, out pos, out dir);
        NetInfo info = instance.m_segments.m_buffer[(int) position.m_segment].Info;
        if (info.m_lanes != null && info.m_lanes.Length > (int) position.m_lane)
            maxSpeed = this.CalculateTargetSpeed(vehicleID, ref vehicleData,
                info.m_lanes[(int) position.m_lane].m_speedLimit, instance.m_lanes.m_buffer[laneID].m_curve);
        else
            maxSpeed = this.CalculateTargetSpeed(vehicleID, ref vehicleData, 1f, 0.0f);
    }

    protected override bool StartPathFind(
        ushort vehicleID,
        ref Vehicle vehicleData,
        Vector3 startPos,
        Vector3 endPos)
    {
        return this.StartPathFind(vehicleID, ref vehicleData, startPos, endPos, true, true, false);
    }

    protected virtual bool StartPathFind(
        ushort vehicleID,
        ref Vehicle vehicleData,
        Vector3 startPos,
        Vector3 endPos,
        bool startBothWays,
        bool endBothWays,
        bool undergroundTarget)
    {
        VehicleInfo info = this.m_info;
        PathUnit.Position pathPosA1;
        PathUnit.Position pathPosB1;
        float distanceSqrA1;
        float distanceSqrB1;
        bool flag1 = PathManager.FindPathPosition(startPos, ItemClass.Service.PublicTransport, NetInfo.LaneType.Vehicle,
            info.m_vehicleType, false, false, 64f, out pathPosA1, out pathPosB1, out distanceSqrA1, out distanceSqrB1);
        PathUnit.Position pathPosA2;
        PathUnit.Position pathPosB2;
        float distanceSqrA2;
        float distanceSqrB2;
        bool flag2 = PathManager.FindPathPosition(endPos, ItemClass.Service.PublicTransport, NetInfo.LaneType.Vehicle,
            info.m_vehicleType, false, false, 64f, out pathPosA2, out pathPosB2, out distanceSqrA2, out distanceSqrB2);
        PathUnit.Position pathPosA3;
        PathUnit.Position pathPosB3;
        float distanceSqrA3;
        float distanceSqrB3;
        if (PathManager.FindPathPosition(startPos, ItemClass.Service.Beautification, NetInfo.LaneType.Vehicle,
            info.m_vehicleType, false, false, 64f, out pathPosA3, out pathPosB3, out distanceSqrA3,
            out distanceSqrB3) && (!flag1 || (double) distanceSqrA3 < (double) distanceSqrA1))
        {
            pathPosA1 = pathPosA3;
            pathPosB1 = pathPosB3;
            distanceSqrA1 = distanceSqrA3;
            distanceSqrB1 = distanceSqrB3;
            flag1 = true;
        }

        PathUnit.Position pathPosA4;
        PathUnit.Position pathPosB4;
        float distanceSqrA4;
        float distanceSqrB4;
        if (PathManager.FindPathPosition(endPos, ItemClass.Service.Beautification, NetInfo.LaneType.Vehicle,
            info.m_vehicleType, false, false, 64f, out pathPosA4, out pathPosB4, out distanceSqrA4,
            out distanceSqrB4) && (!flag2 || (double) distanceSqrA4 < (double) distanceSqrA2))
        {
            pathPosA2 = pathPosA4;
            pathPosB2 = pathPosB4;
            distanceSqrA2 = distanceSqrA4;
            distanceSqrB2 = distanceSqrB4;
            flag2 = true;
        }

        if (flag1 && flag2)
        {
            if (!startBothWays || (double) distanceSqrA1 < 10.0)
                pathPosB1 = new PathUnit.Position();
            if (!endBothWays || (double) distanceSqrA2 < 10.0)
                pathPosB2 = new PathUnit.Position();
            uint unit;
            if (Singleton<PathManager>.instance.CreatePath(out unit,
                ref Singleton<SimulationManager>.instance.m_randomizer,
                Singleton<SimulationManager>.instance.m_currentBuildIndex, pathPosA1, pathPosB1, pathPosA2, pathPosB2,
                NetInfo.LaneType.Vehicle, info.m_vehicleType, 20000f, false,
                this.IgnoreBlocked(vehicleID, ref vehicleData), false, false))
            {
                if (vehicleData.m_path != 0U)
                    Singleton<PathManager>.instance.ReleasePath(vehicleData.m_path);
                vehicleData.m_path = unit;
                vehicleData.m_flags |= Vehicle.Flags.WaitingPath;
                return true;
            }
        }

        return false;
    }

    public override bool TrySpawn(ushort vehicleID, ref Vehicle vehicleData)
    {
        if ((vehicleData.m_flags & Vehicle.Flags.Spawned) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted |
                                                               Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                                                               Vehicle.Flags.TransferToTarget |
                                                               Vehicle.Flags.TransferToSource |
                                                               Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                                               Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                                                               Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                                                               Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                                               Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                                                               Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                                                               Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget |
                                                               Vehicle.Flags.Importing | Vehicle.Flags.Exporting |
                                                               Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                                               Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
                                                               Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                                                               Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                                               Vehicle.Flags.InsideBuilding |
                                                               Vehicle.Flags.LeftHandDrive))
            return true;
        if (CargoFerryAI.CheckOverlap(vehicleData.m_segment, (ushort) 0, 1000f))
        {
            vehicleData.m_flags |= Vehicle.Flags.WaitingSpace;
            return false;
        }

        vehicleData.Spawn(vehicleID);
        vehicleData.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                               Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
                               Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                               Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                               Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                               Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingCargo |
                               Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                               Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                               Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                               Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                               Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
        return true;
    }
}
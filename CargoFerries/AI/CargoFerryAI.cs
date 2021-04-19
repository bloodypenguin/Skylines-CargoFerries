using ColossalFramework;
using ColossalFramework.Math;
using System;
using UnityEngine;

//based of FerryAI + parts from CargoShipAI
public class CargoFerryAI : FerryAI
{
    [CustomizableProperty("Cargo capacity")]
    public int m_cargoCapacity = 1;
    
    public TransportInfo m_transportInfo;

    public override void CreateVehicle(ushort vehicleID, ref Vehicle data)
    {
        base.CreateVehicle(vehicleID, ref data);
        data.m_flags |= Vehicle.Flags.WaitingTarget;
        data.m_flags |= Vehicle.Flags.WaitingCargo;
        data.m_flags |= Vehicle.Flags.WaitingLoading;
        data.m_flags |= Vehicle.Flags.Stopped;
    }
    
    public override void LoadVehicle(ushort vehicleID, ref Vehicle data)
    {
      base.LoadVehicle(vehicleID, ref data);

      LoadVehicle_ShipAI(vehicleID, ref data);
      
      if (data.m_sourceBuilding != (ushort) 0)
        Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) data.m_sourceBuilding].AddOwnVehicle(vehicleID, ref data);
      if (data.m_targetBuilding == (ushort) 0)
        return;
      Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) data.m_targetBuilding].AddGuestVehicle(vehicleID, ref data);
    }

    private void LoadVehicle_ShipAI(ushort vehicleID, ref Vehicle data)
    {
      if (data.m_path != 0U || (data.m_flags & Vehicle.Flags.WaitingPath) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
        return;
      data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
    }
    
      public override void SetSource(ushort vehicleID, ref Vehicle data, ushort sourceBuilding)
  {
    this.RemoveSource(vehicleID, ref data);
    data.m_sourceBuilding = sourceBuilding;
    if (sourceBuilding == (ushort) 0)
      return;
    data.Unspawn(vehicleID);
    BuildingManager instance = Singleton<BuildingManager>.instance;
    Vector3 position;
    Vector3 target;
    instance.m_buildings.m_buffer[(int) sourceBuilding].Info.m_buildingAI.CalculateSpawnPosition(sourceBuilding, ref instance.m_buildings.m_buffer[(int) sourceBuilding], ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info, out position, out target);
    Quaternion rotation = Quaternion.identity;
    Vector3 forward = target - position;
    if ((double) forward.sqrMagnitude > 0.00999999977648258)
      rotation = Quaternion.LookRotation(forward);
    data.m_frame0 = new Vehicle.Frame(position, rotation);
    data.m_frame1 = data.m_frame0;
    data.m_frame2 = data.m_frame0;
    data.m_frame3 = data.m_frame0;
    data.m_targetPos0 = (Vector4) (position + Vector3.ClampMagnitude(target - position, 0.5f));
    data.m_targetPos0.w = 2f;
    data.m_targetPos1 = data.m_targetPos0;
    data.m_targetPos2 = data.m_targetPos0;
    data.m_targetPos3 = data.m_targetPos0;
    this.FrameDataUpdated(vehicleID, ref data, ref data.m_frame0);
    Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) sourceBuilding].AddOwnVehicle(vehicleID, ref data);
    if ((Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) sourceBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.None)
      return;
    if ((data.m_flags & Vehicle.Flags.TransferToTarget) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
    {
      data.m_flags |= Vehicle.Flags.Importing;
    }
    else
    {
      if ((data.m_flags & Vehicle.Flags.TransferToSource) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
        return;
      data.m_flags |= Vehicle.Flags.Exporting;
    }
  }
    
    public override void SetTarget(ushort vehicleID, ref Vehicle data, ushort targetBuilding)
    {
      if ((int) targetBuilding != (int) data.m_targetBuilding)
      {
        this.RemoveTarget(vehicleID, ref data);
        data.m_targetBuilding = targetBuilding;
        data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
        data.m_waitCounter = (byte) 0;
        if (targetBuilding != (ushort) 0)
        {
          Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) targetBuilding].AddGuestVehicle(vehicleID, ref data);
          if ((Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) targetBuilding].m_flags & Building.Flags.IncomingOutgoing) != Building.Flags.None)
          {
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
              data.m_flags |= Vehicle.Flags.Exporting;
            else if ((data.m_flags & Vehicle.Flags.TransferToSource) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
              data.m_flags |= Vehicle.Flags.Importing;
          }
        }
        else
          data.m_flags |= Vehicle.Flags.GoingBack;
      }
      if ((data.m_flags & Vehicle.Flags.WaitingCargo) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) || this.StartPathFind(vehicleID, ref data))
        return;
      data.Unspawn(vehicleID);
    }
    
    private void RemoveTarget(ushort vehicleID, ref Vehicle data)
    {
      if (data.m_targetBuilding == (ushort) 0)
        return;
      Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) data.m_targetBuilding].RemoveGuestVehicle(vehicleID, ref data);
      data.m_targetBuilding = (ushort) 0;
    }
    
    public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
    {
      if ((data.m_flags & Vehicle.Flags.WaitingCargo) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
      {
        bool flag = Singleton<SimulationManager>.instance.m_randomizer.Int32(2U) == 0;
        if (!flag && data.m_sourceBuilding != (ushort) 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) data.m_sourceBuilding].m_flags & Building.Flags.Active) == Building.Flags.None)
          flag = true;
        if (!flag && data.m_targetBuilding != (ushort) 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) data.m_targetBuilding].m_flags & Building.Flags.Active) == Building.Flags.None)
          flag = true;
        if (!flag)
        {
          data.m_waitCounter = (int) data.m_transferSize < this.m_cargoCapacity ? (byte) Mathf.Min((int) data.m_waitCounter + 1, (int) byte.MaxValue) : byte.MaxValue;
          if (data.m_waitCounter == byte.MaxValue && ((data.m_flags & Vehicle.Flags.Spawned) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) || this.CanSpawnAt(data.GetLastFramePosition())))
          {
            data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
            data.m_flags |= Vehicle.Flags.Leaving;
            data.m_waitCounter = (byte) 0;
            this.StartPathFind(vehicleID, ref data);
          }
        }
      }
      else if ((data.m_flags & Vehicle.Flags.Stopped) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
      {
        if ((data.m_flags & Vehicle.Flags.Spawned) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) && ++data.m_waitCounter == (byte) 16)
        {
          data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
          data.m_flags |= Vehicle.Flags.Leaving;
          data.m_waitCounter = (byte) 0;
        }
      }
      else if ((data.m_flags & Vehicle.Flags.GoingBack) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) && data.m_targetBuilding != (ushort) 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) data.m_targetBuilding].m_flags & Building.Flags.Active) == Building.Flags.None)
        this.SetTarget(vehicleID, ref data, (ushort) 0);
      base.SimulationStep(vehicleID, ref data, physicsLodRefPos);
    }
    
    
    public override void UpdateBuildingTargetPositions(
      ushort vehicleID,
      ref Vehicle vehicleData,
      Vector3 refPos,
      ushort leaderID,
      ref Vehicle leaderData,
      ref int index,
      float minSqrDistance)
    {
        //do nothing
    }
    
    public override void ReleaseVehicle(ushort vehicleID, ref Vehicle data)
    {
      this.RemoveSource(vehicleID, ref data);
      this.RemoveTarget(vehicleID, ref data);
      base.ReleaseVehicle(vehicleID, ref data);
      UnityEngine.Debug.Log(new System.Diagnostics.StackTrace().ToString());
    }
    
    private void RemoveSource(ushort vehicleID, ref Vehicle data)
    {
      if (data.m_sourceBuilding == (ushort) 0)
        return;
      Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) data.m_sourceBuilding].RemoveOwnVehicle(vehicleID, ref data);
      data.m_sourceBuilding = (ushort) 0;
    }
    
      public override bool ArriveAtDestination(ushort vehicleID, ref Vehicle vehicleData)
  {
    if ((vehicleData.m_flags & Vehicle.Flags.WaitingTarget) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
      return false;
    if ((vehicleData.m_flags & Vehicle.Flags.WaitingLoading) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
    {
      vehicleData.m_waitCounter = (byte) Mathf.Min((int) vehicleData.m_waitCounter + 1, (int) byte.MaxValue);
      if (vehicleData.m_waitCounter < (byte) 16)
        return false;
      if (vehicleData.m_targetBuilding != (ushort) 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) vehicleData.m_targetBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.None)
      {
        ushort nextCargoParent = CargoTruckAI.FindNextCargoParent(vehicleData.m_targetBuilding, this.m_info.m_class.m_service, this.m_info.m_class.m_subService);
        if (nextCargoParent != (ushort) 0)
        {
          ushort targetBuilding = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) nextCargoParent].m_targetBuilding;
          if (targetBuilding != (ushort) 0)
          {
            CargoTruckAI.SwitchCargoParent(nextCargoParent, vehicleID);
            vehicleData.m_waitCounter = (byte) 0;
            vehicleData.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
            this.SetTarget(vehicleID, ref vehicleData, targetBuilding);
            return (vehicleData.m_flags & Vehicle.Flags.Spawned) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive);
          }
        }
      }
      Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
      return true;
    }
    return (vehicleData.m_flags & Vehicle.Flags.GoingBack) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) ? this.ArriveAtSource(vehicleID, ref vehicleData) : this.ArriveAtTarget(vehicleID, ref vehicleData);
  }
      
      private bool ArriveAtSource(ushort vehicleID, ref Vehicle data)
      {
        VehicleManager instance = Singleton<VehicleManager>.instance;
        ushort vehicle = data.m_firstCargo;
        data.m_firstCargo = (ushort) 0;
        int num = 0;
        while (vehicle != (ushort) 0)
        {
          ushort nextCargo = instance.m_vehicles.m_buffer[(int) vehicle].m_nextCargo;
          instance.m_vehicles.m_buffer[(int) vehicle].m_nextCargo = (ushort) 0;
          instance.m_vehicles.m_buffer[(int) vehicle].m_cargoParent = (ushort) 0;
          instance.ReleaseVehicle(vehicle);
          vehicle = nextCargo;
          if (++num > 16384)
          {
            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
            break;
          }
        }
        data.m_waitCounter = (byte) 0;
        data.m_flags |= Vehicle.Flags.WaitingLoading;
        return false;
      }
    
      private bool ArriveAtTarget(ushort vehicleID, ref Vehicle data)
      {
        VehicleManager instance = Singleton<VehicleManager>.instance;
        ushort vehicleID1 = data.m_firstCargo;
        data.m_firstCargo = (ushort) 0;
        int num = 0;
        while (vehicleID1 != (ushort) 0)
        {
          ushort nextCargo = instance.m_vehicles.m_buffer[(int) vehicleID1].m_nextCargo;
          instance.m_vehicles.m_buffer[(int) vehicleID1].m_nextCargo = (ushort) 0;
          instance.m_vehicles.m_buffer[(int) vehicleID1].m_cargoParent = (ushort) 0;
          VehicleInfo info = instance.m_vehicles.m_buffer[(int) vehicleID1].Info;
          if (data.m_targetBuilding != (ushort) 0)
          {
            info.m_vehicleAI.SetSource(vehicleID1, ref instance.m_vehicles.m_buffer[(int) vehicleID1], data.m_targetBuilding);
            info.m_vehicleAI.SetTarget(vehicleID1, ref instance.m_vehicles.m_buffer[(int) vehicleID1], instance.m_vehicles.m_buffer[(int) vehicleID1].m_targetBuilding);
          }
          vehicleID1 = nextCargo;
          if (++num > 16384)
          {
            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
            break;
          }
        }
        data.m_waitCounter = (byte) 0;
        data.m_flags |= Vehicle.Flags.WaitingLoading;
        return false;
      }

  protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData)
  {
    if (vehicleData.m_leadingVehicle == (ushort) 0)
    {
      Vector3 startPos = (vehicleData.m_flags & Vehicle.Flags.Reversed) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) ? (Vector3) vehicleData.m_targetPos0 : (Vector3) Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) vehicleData.GetLastVehicle(vehicleID)].m_targetPos0;
      if ((vehicleData.m_flags & Vehicle.Flags.GoingBack) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
      {
        if (vehicleData.m_sourceBuilding != (ushort) 0)
        {
          BuildingManager instance = Singleton<BuildingManager>.instance;
          BuildingInfo info = instance.m_buildings.m_buffer[(int) vehicleData.m_sourceBuilding].Info;
          Randomizer randomizer = new Randomizer((int) vehicleID);
          Vector3 position;
          info.m_buildingAI.CalculateSpawnPosition(vehicleData.m_sourceBuilding, ref instance.m_buildings.m_buffer[(int) vehicleData.m_sourceBuilding], ref randomizer, this.m_info, out position, out Vector3 _);
          var startPathFind = this.StartPathFind(vehicleID, ref vehicleData, startPos, position);
          return startPathFind;
        }
      }
      else if (vehicleData.m_targetBuilding != (ushort) 0)
      {
        BuildingManager instance = Singleton<BuildingManager>.instance;
        BuildingInfo info = instance.m_buildings.m_buffer[(int) vehicleData.m_targetBuilding].Info;
        Randomizer randomizer = new Randomizer((int) vehicleID);
        Vector3 position;
        info.m_buildingAI.CalculateSpawnPosition(vehicleData.m_targetBuilding, ref instance.m_buildings.m_buffer[(int) vehicleData.m_targetBuilding], ref randomizer, this.m_info, out position, out Vector3 _);
        var startPathFind = this.StartPathFind(vehicleID, ref vehicleData, startPos, position);
        return startPathFind;
      }
    }
    return false;
  }

  public override bool TrySpawn(ushort vehicleID, ref Vehicle vehicleData)
  {
    if ((vehicleData.m_flags & Vehicle.Flags.Spawned) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
      return true;
    if (CheckOverlap(vehicleData.m_segment, (ushort) 0))
    {
      vehicleData.m_flags |= Vehicle.Flags.WaitingSpace;
      return false;
    }
    PathUnit.Position position;
    if (vehicleData.m_path != 0U && Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].GetPosition(0, out position))
    {
      uint laneId = PathManager.GetLaneID(position);
      if (laneId != 0U && !Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].CheckSpace(1000f, vehicleID))
      {
        vehicleData.m_flags |= Vehicle.Flags.WaitingSpace;
        return false;
      }
    }
    vehicleData.Spawn(vehicleID);
    vehicleData.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
    return true;
  }
    
    private static bool CheckOverlap(Segment3 segment, ushort ignoreVehicle)
    {
      VehicleManager instance = Singleton<VehicleManager>.instance;
      Vector3 vector3_1 = segment.Min();
      Vector3 vector3_2 = segment.Max();
      int num1 = Mathf.Max((int) (((double) vector3_1.x - 100.0) / 320.0 + 27.0), 0);
      int num2 = Mathf.Max((int) (((double) vector3_1.z - 100.0) / 320.0 + 27.0), 0);
      int num3 = Mathf.Min((int) (((double) vector3_2.x + 100.0) / 320.0 + 27.0), 53);
      int num4 = Mathf.Min((int) (((double) vector3_2.z + 100.0) / 320.0 + 27.0), 53);
      bool overlap = false;
      for (int index1 = num2; index1 <= num4; ++index1)
      {
        for (int index2 = num1; index2 <= num3; ++index2)
        {
          ushort otherID = instance.m_vehicleGrid2[index1 * 54 + index2];
          int num5 = 0;
          while (otherID != (ushort) 0)
          {
            otherID = CheckOverlap(segment, ignoreVehicle, otherID, ref instance.m_vehicles.m_buffer[(int) otherID], ref overlap);
            if (++num5 > 16384)
            {
              CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
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
      ushort otherID,
      ref Vehicle otherData,
      ref bool overlap)
    {
      if ((ignoreVehicle == (ushort) 0 || (int) otherID != (int) ignoreVehicle && (int) otherData.m_leadingVehicle != (int) ignoreVehicle && (int) otherData.m_trailingVehicle != (int) ignoreVehicle) && (double) segment.DistanceSqr(otherData.m_segment, out float _, out float _) < 400.0)
        overlap = true;
      return otherData.m_nextGridVehicle;
    }

    //UI stuff
    
      public override Color GetColor(
    ushort vehicleID,
    ref Vehicle data,
    InfoManager.InfoMode infoMode)
  {
    switch (infoMode)
    {
      case InfoManager.InfoMode.Transport:
        return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int) this.m_transportInfo.m_transportType];
      case InfoManager.InfoMode.Connections:
        InfoManager.SubInfoMode currentSubMode = Singleton<InfoManager>.instance.CurrentSubMode;
        TransferManager.TransferReason transferType = (TransferManager.TransferReason) data.m_transferType;
        if (currentSubMode == InfoManager.SubInfoMode.Default && (data.m_flags & Vehicle.Flags.Importing) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) && transferType != TransferManager.TransferReason.None)
          return Singleton<TransferManager>.instance.m_properties.m_resourceColors[(int) transferType];
        return currentSubMode == InfoManager.SubInfoMode.WaterPower && (data.m_flags & Vehicle.Flags.Exporting) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) && transferType != TransferManager.TransferReason.None ? Singleton<TransferManager>.instance.m_properties.m_resourceColors[(int) transferType] : Singleton<InfoManager>.instance.m_properties.m_neutralColor;
      case InfoManager.InfoMode.TrafficRoutes:
        if (Singleton<InfoManager>.instance.CurrentSubMode == InfoManager.SubInfoMode.Default)
        {
          InstanceID empty = InstanceID.Empty;
          empty.Vehicle = vehicleID;
          return Singleton<NetManager>.instance.PathVisualizer.IsPathVisible(empty) ? Singleton<InfoManager>.instance.m_properties.m_routeColors[3] : Singleton<InfoManager>.instance.m_properties.m_neutralColor;
        }
        break;
    }
    return base.GetColor(vehicleID, ref data, infoMode);
  }

  public override string GetLocalizedStatus(
    ushort vehicleID,
    ref Vehicle data,
    out InstanceID target)
  {
    if ((data.m_flags & Vehicle.Flags.WaitingCargo) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
    {
      target = InstanceID.Empty;
      return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOSHIP_LOADING");
    }
    if ((data.m_flags & Vehicle.Flags.GoingBack) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
    {
      target = InstanceID.Empty;
      return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOTRUCK_RETURN");
    }
    if (data.m_targetBuilding != (ushort) 0)
    {
      target = InstanceID.Empty;
      target.Building = data.m_targetBuilding;
      return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOSHIP_TRANSPORT");
    }
    target = InstanceID.Empty;
    return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CONFUSED");
  }

  public override string GetLocalizedStatus(
    ushort parkedVehicleID,
    ref VehicleParked data,
    out InstanceID target)
  {
    target = InstanceID.Empty;
    return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOSHIP_LOADING");
  }
    
    public override void GetBufferStatus(
      ushort vehicleID,
      ref Vehicle data,
      out string localeKey,
      out int current,
      out int max)
    {
      localeKey = "Default";
      current = 0;
      max = this.m_cargoCapacity;
      VehicleManager instance = Singleton<VehicleManager>.instance;
      ushort num1 = data.m_firstCargo;
      int num2 = 0;
      while (num1 != (ushort) 0)
      {
        ++current;
        num1 = instance.m_vehicles.m_buffer[(int) num1].m_nextCargo;
        if (++num2 > 16384)
        {
          CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
          break;
        }
      }
      if ((data.m_flags & Vehicle.Flags.DummyTraffic) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
        return;
      Randomizer randomizer = new Randomizer((int) vehicleID);
      current = randomizer.Int32(max >> 1, max);
    }
}
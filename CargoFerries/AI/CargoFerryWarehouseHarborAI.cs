using System;
using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Math;
using UnityEngine;

namespace CargoFerries.AI
{
    //based of CargoHarborAI but without animals, connections & checking height
    public class CargoFerryWarehouseHarborAI : CargoFerryHarborAI
    {
        [CustomizableProperty("Truck Count")] public int m_truckCount = 25;

        [CustomizableProperty("Storage Capacity")]
        public int m_storageCapacity = 350000;

        [CustomizableProperty("Storage Type")]
        public TransferManager.TransferReason m_storageType = TransferManager.TransferReason.None;

        protected override string GetLocalizedStatusActive(ushort buildingID, ref Building data)
        {
            if (this.IsFull(buildingID, ref data))
                return ColossalFramework.Globalization.Locale.Get("BUILDING_STATUS_FULL");
            return (data.m_flags & Building.Flags.Downgrading) != Building.Flags.None
                ? ColossalFramework.Globalization.Locale.Get("BUILDING_STATUS_EMPTYING")
                : base.GetLocalizedStatusActive(buildingID, ref data);
        }

        public override void CreateBuilding(ushort buildingID, ref Building data)
        {
            base.CreateBuilding(buildingID, ref data);
            data.m_seniors = byte.MaxValue;
            data.m_adults = byte.MaxValue;
            data.m_flags |= Building.Flags.Downgrading;
            if (this.GetTransferReason(buildingID, ref data) != TransferManager.TransferReason.None)
                return;
            data.m_problems = Notification.AddProblems(data.m_problems, Notification.Problem.ResourceNotSelected);
        }

        public override void SimulationStep(
            ushort buildingID,
            ref Building buildingData,
            ref Building.Frame frameData)
        {
            base.SimulationStep(buildingID, ref buildingData, ref frameData);
            this.CheckCapacity(buildingID, ref buildingData);
            if ((Singleton<SimulationManager>.instance.m_currentFrameIndex & 4095U) < 3840U)
                return;
            buildingData.m_finalExport = buildingData.m_tempExport;
            buildingData.m_finalImport = buildingData.m_tempImport;
            buildingData.m_tempExport = (byte) 0;
            buildingData.m_tempImport = (byte) 0;
        }

        public override void StartTransfer(
            ushort buildingID,
            ref Building data,
            TransferManager.TransferReason material,
            TransferManager.TransferOffer offer)
        {
            if (material != this.GetActualTransferReason(buildingID, ref data))
            {
                base.StartTransfer(buildingID, ref data, material, offer);
            }
            else
            {
                VehicleInfo transferVehicleService;
                if (material == TransferManager.TransferReason.Fish)
                {
                    transferVehicleService = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(
                        ref Singleton<SimulationManager>.instance.m_randomizer,
                        ItemClass.Service.Fishing,
                        ItemClass.SubService.None, ItemClass.Level.Level1, VehicleInfo.VehicleType.Car);
                }
                else
                {
                    transferVehicleService = WarehouseAI.GetTransferVehicleService(material,
                        ItemClass.Level.Level1, ref Singleton<SimulationManager>.instance.m_randomizer);     
                }

                if (transferVehicleService == null)
                    return;
                Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                ushort vehicle;
                if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle,
                    ref Singleton<SimulationManager>.instance.m_randomizer, transferVehicleService, data.m_position,
                    material, false, true))
                    return;
                transferVehicleService.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int) vehicle], buildingID);
                transferVehicleService.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int) vehicle],
                    material, offer);
                VehicleManager.instance.m_vehicles.m_buffer[vehicle].m_touristCount = 1; //to indicate that it's really own truck
                ushort building = offer.Building;
                if (building != (ushort) 0 &&
                    (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) building].m_flags &
                     Building.Flags.IncomingOutgoing) != Building.Flags.None)
                {
                    int size;
                    transferVehicleService.m_vehicleAI.GetSize(vehicle, ref vehicles.m_buffer[(int) vehicle], out size,
                        out int _);
                    CommonBuildingAI.ExportResource(buildingID, ref data, material, size);
                }

                data.m_outgoingProblemTimer = (byte) 0;
            }
        }

        public override void ModifyMaterialBuffer(
            ushort buildingID,
            ref Building data,
            TransferManager.TransferReason material,
            ref int amountDelta)
        {
            if (material == this.GetActualTransferReason(buildingID, ref data))
            {
                int num = (int) data.m_customBuffer1 * 100;
                amountDelta = Mathf.Clamp(amountDelta, -num, this.m_storageCapacity - num);
                data.m_customBuffer1 = (ushort) ((num + amountDelta) / 100);
            }
            else
                base.ModifyMaterialBuffer(buildingID, ref data, material, ref amountDelta);
        }

        public override void BuildingDeactivated(ushort buildingID, ref Building data)
        {
            TransferManager.TransferReason actualTransferReason = this.GetActualTransferReason(buildingID, ref data);
            if (actualTransferReason != TransferManager.TransferReason.None)
            {
                TransferManager.TransferOffer offer = new TransferManager.TransferOffer();
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveIncomingOffer(actualTransferReason, offer);
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(actualTransferReason, offer);
            }

            base.BuildingDeactivated(buildingID, ref data);
        }

        private void CheckCapacity(ushort buildingID, ref Building buildingData)
        {
            int num = (int) buildingData.m_customBuffer1 * 100;
            if (num * 3 >= this.m_storageCapacity * 2)
            {
                if ((buildingData.m_flags & Building.Flags.CapacityFull) == Building.Flags.CapacityFull)
                    return;
                buildingData.m_flags |= Building.Flags.CapacityFull;
            }
            else if (num * 3 >= this.m_storageCapacity)
            {
                if ((buildingData.m_flags & Building.Flags.CapacityFull) == Building.Flags.CapacityStep2)
                    return;
                buildingData.m_flags = buildingData.m_flags & (Building.Flags.ContentMask |
                                                               Building.Flags.IncomingOutgoing |
                                                               Building.Flags.Created | Building.Flags.Deleted |
                                                               Building.Flags.Original | Building.Flags.CustomName |
                                                               Building.Flags.Untouchable | Building.Flags.FixedHeight |
                                                               Building.Flags.RateReduced | Building.Flags.HighDensity |
                                                               Building.Flags.RoadAccessFailed |
                                                               Building.Flags.Evacuating | Building.Flags.Completed |
                                                               Building.Flags.Active | Building.Flags.Abandoned |
                                                               Building.Flags.Demolishing |
                                                               Building.Flags.ZonesUpdated |
                                                               Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                               Building.Flags.Upgrading |
                                                               Building.Flags.SecondaryLoading | Building.Flags.Hidden |
                                                               Building.Flags.EventActive | Building.Flags.Flooded |
                                                               Building.Flags.Filling | Building.Flags.Historical) |
                                       Building.Flags.CapacityStep2;
            }
            else if (num >= this.GetMaxLoadSize())
            {
                if ((buildingData.m_flags & Building.Flags.CapacityFull) == Building.Flags.CapacityStep1)
                    return;
                buildingData.m_flags = buildingData.m_flags & (Building.Flags.ContentMask |
                                                               Building.Flags.IncomingOutgoing |
                                                               Building.Flags.Created | Building.Flags.Deleted |
                                                               Building.Flags.Original | Building.Flags.CustomName |
                                                               Building.Flags.Untouchable | Building.Flags.FixedHeight |
                                                               Building.Flags.RateReduced | Building.Flags.HighDensity |
                                                               Building.Flags.RoadAccessFailed |
                                                               Building.Flags.Evacuating | Building.Flags.Completed |
                                                               Building.Flags.Active | Building.Flags.Abandoned |
                                                               Building.Flags.Demolishing |
                                                               Building.Flags.ZonesUpdated |
                                                               Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                               Building.Flags.Upgrading |
                                                               Building.Flags.SecondaryLoading | Building.Flags.Hidden |
                                                               Building.Flags.EventActive | Building.Flags.Flooded |
                                                               Building.Flags.Filling | Building.Flags.Historical) |
                                       Building.Flags.CapacityStep1;
            }
            else
            {
                if ((buildingData.m_flags & Building.Flags.CapacityFull) == Building.Flags.None)
                    return;
                buildingData.m_flags &= Building.Flags.ContentMask | Building.Flags.IncomingOutgoing |
                                        Building.Flags.Created | Building.Flags.Deleted | Building.Flags.Original |
                                        Building.Flags.CustomName | Building.Flags.Untouchable |
                                        Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                        Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                        Building.Flags.Evacuating | Building.Flags.Completed | Building.Flags.Active |
                                        Building.Flags.Abandoned | Building.Flags.Demolishing |
                                        Building.Flags.ZonesUpdated | Building.Flags.Downgrading |
                                        Building.Flags.Collapsed | Building.Flags.Upgrading |
                                        Building.Flags.SecondaryLoading | Building.Flags.Hidden |
                                        Building.Flags.EventActive | Building.Flags.Flooded | Building.Flags.Filling |
                                        Building.Flags.Historical;
            }
        }

        protected override void ProduceGoods(
            ushort buildingID,
            ref Building buildingData,
            ref Building.Frame frameData,
            int productionRate,
            int finalProductionRate,
            ref Citizen.BehaviourData behaviour,
            int aliveWorkerCount,
            int totalWorkerCount,
            int workPlaceCount,
            int aliveVisitorCount,
            int totalVisitorCount,
            int visitPlaceCount)
        {
            DistrictManager instance = Singleton<DistrictManager>.instance;
            byte num1 = instance.GetPark(buildingData.m_position);
            if (num1 != (byte) 0 && !instance.m_parks.m_buffer[(int) num1].IsIndustry)
                num1 = (byte) 0;
            if (finalProductionRate != 0)
            {
                this.HandleDead(buildingID, ref buildingData, ref behaviour, totalWorkerCount);
                TransferManager.TransferReason actualTransferReason =
                    this.GetActualTransferReason(buildingID, ref buildingData);
                TransferManager.TransferReason transferReason = this.GetTransferReason(buildingID, ref buildingData);
                if (actualTransferReason != TransferManager.TransferReason.None)
                {
                    int maxLoadSize = this.GetMaxLoadSize();
                    bool flag1 = this.IsFull(buildingID, ref buildingData);
                    int num2 = finalProductionRate;
                    int amount = (int) buildingData.m_customBuffer1 * 100;
                    int num3 = (num2 * this.m_truckCount + 99) / 100;
                    int count1 = 0;
                    int cargo1 = 0;
                    int capacity1 = 0;
                    int outside1 = 0;
                    this.CalculateOwnVehicles(buildingID, ref buildingData, actualTransferReason, ref count1,
                        ref cargo1, ref capacity1, ref outside1);
                    buildingData.m_tempExport =
                        (byte) Mathf.Clamp(outside1, (int) buildingData.m_tempExport, (int) byte.MaxValue);
                    int count2 = 0;
                    int cargo2 = 0;
                    int capacity2 = 0;
                    int outside2 = 0;
                    this.CalculateGuestVehicles(buildingID, ref buildingData, actualTransferReason, ref count2,
                        ref cargo2, ref capacity2, ref outside2);
                    buildingData.m_tempImport =
                        (byte) Mathf.Clamp(outside2, (int) buildingData.m_tempImport, (int) byte.MaxValue);
                    if (num1 != (byte) 0)
                        instance.m_parks.m_buffer[(int) num1].AddBufferStatus(actualTransferReason, amount, cargo2,
                            this.m_storageCapacity);
                    if (transferReason != actualTransferReason)
                    {
                        if (amount > 0 && count1 < num3)
                            Singleton<TransferManager>.instance.AddOutgoingOffer(actualTransferReason,
                                new TransferManager.TransferOffer()
                                {
                                    Priority = 8,
                                    Building = buildingID,
                                    Position = buildingData.m_position,
                                    Amount = Mathf.Min(Mathf.Max(1, amount / Mathf.Max(1, maxLoadSize)), num3 - count1),
                                    Active = true,
                                    Exclude = true
                                });
                    }
                    else
                    {
                        if (amount >= maxLoadSize && count1 < num3)
                            Singleton<TransferManager>.instance.AddOutgoingOffer(actualTransferReason,
                                new TransferManager.TransferOffer()
                                {
                                    Priority = (buildingData.m_flags & Building.Flags.Filling) == Building.Flags.None
                                        ? ((buildingData.m_flags & Building.Flags.Downgrading) == Building.Flags.None
                                            ? Mathf.Clamp(amount / Mathf.Max(1, this.m_storageCapacity >> 2) - 1, 0, 2)
                                            : Mathf.Clamp(amount / Mathf.Max(1, this.m_storageCapacity >> 2) + 2, 0, 2))
                                        : 0,
                                    Building = buildingID,
                                    Position = buildingData.m_position,
                                    Amount = Mathf.Min(amount / Mathf.Max(1, maxLoadSize), num3 - count1),
                                    Active = true,
                                    Exclude = true
                                });
                        int num4 = amount + cargo2;
                        if (num4 < this.m_storageCapacity)
                            Singleton<TransferManager>.instance.AddIncomingOffer(actualTransferReason,
                                new TransferManager.TransferOffer()
                                {
                                    Priority = (buildingData.m_flags & Building.Flags.Filling) == Building.Flags.None
                                        ? ((buildingData.m_flags & Building.Flags.Downgrading) == Building.Flags.None
                                            ? Mathf.Clamp(
                                                (this.m_storageCapacity - num4) /
                                                Mathf.Max(1, this.m_storageCapacity >> 2) - 1, 0, 2)
                                            : 0)
                                        : Mathf.Clamp(
                                            (this.m_storageCapacity - num4) /
                                            Mathf.Max(1, this.m_storageCapacity >> 2) + 1, 0, 2),
                                    Building = buildingID,
                                    Position = buildingData.m_position,
                                    Amount = Mathf.Max(1, (this.m_storageCapacity - num4) / Mathf.Max(1, maxLoadSize)),
                                    Active = false,
                                    Exclude = true
                                });
                    }

                    bool flag2 = this.IsFull(buildingID, ref buildingData);
                    if (flag1 != flag2)
                    {
                        // if (flag2)
                        // {
                        //     if (this.m_fullPassMilestone != null)
                        //         this.m_fullPassMilestone.Unlock();
                        // }
                        // else if (this.m_fullPassMilestone != null)
                        //     this.m_fullPassMilestone.Relock();
                    }
                }

                if (actualTransferReason != transferReason && buildingData.m_customBuffer1 == (ushort) 0)
                {
                    buildingData.m_adults = buildingData.m_seniors;
                    this.SetContentFlags(buildingID, ref buildingData, transferReason);
                }

                int rate = finalProductionRate * this.m_noiseAccumulation / 100;
                if (rate != 0)
                    Singleton<ImmaterialResourceManager>.instance.AddResource(
                        ImmaterialResourceManager.Resource.NoisePollution, rate, buildingData.m_position,
                        this.m_noiseRadius);
            }

            base.ProduceGoods(buildingID, ref buildingData, ref frameData, productionRate, finalProductionRate,
                ref behaviour, aliveWorkerCount, totalWorkerCount, workPlaceCount, aliveVisitorCount, totalVisitorCount,
                visitPlaceCount);
        }

        public override string GetDebugString(ushort buildingID, ref Building data)
        {
            TransferManager.TransferReason actualTransferReason = this.GetActualTransferReason(buildingID, ref data);
            if (actualTransferReason == TransferManager.TransferReason.None)
                return base.GetDebugString(buildingID, ref data);
            int count = 0;
            int cargo = 0;
            int capacity = 0;
            int outside = 0;
            this.CalculateGuestVehicles(buildingID, ref data, actualTransferReason, ref count, ref cargo, ref capacity,
                ref outside);
            int num = (int) data.m_customBuffer1 * 100;
            return StringUtils.SafeFormat("{0}\n{1}: {2} (+{3})", (object) base.GetDebugString(buildingID, ref data),
                (object) actualTransferReason, (object) num, (object) cargo);
        }

        public override void SetEmptying(ushort buildingID, ref Building data, bool emptying)
        {
            if (emptying)
                data.m_flags = data.m_flags & (Building.Flags.ContentMask | Building.Flags.IncomingOutgoing |
                                               Building.Flags.CapacityFull | Building.Flags.Created |
                                               Building.Flags.Deleted | Building.Flags.Original |
                                               Building.Flags.CustomName | Building.Flags.Untouchable |
                                               Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                               Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                               Building.Flags.Evacuating | Building.Flags.Completed |
                                               Building.Flags.Active | Building.Flags.Abandoned |
                                               Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                               Building.Flags.Downgrading | Building.Flags.Collapsed |
                                               Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                               Building.Flags.Hidden | Building.Flags.EventActive |
                                               Building.Flags.Flooded | Building.Flags.Historical) |
                               Building.Flags.Downgrading;
            else
                data.m_flags &= Building.Flags.ContentMask | Building.Flags.IncomingOutgoing |
                                Building.Flags.CapacityFull | Building.Flags.Created | Building.Flags.Deleted |
                                Building.Flags.Original | Building.Flags.CustomName | Building.Flags.Untouchable |
                                Building.Flags.FixedHeight | Building.Flags.RateReduced | Building.Flags.HighDensity |
                                Building.Flags.RoadAccessFailed | Building.Flags.Evacuating | Building.Flags.Completed |
                                Building.Flags.Active | Building.Flags.Abandoned | Building.Flags.Demolishing |
                                Building.Flags.ZonesUpdated | Building.Flags.Collapsed | Building.Flags.Upgrading |
                                Building.Flags.SecondaryLoading | Building.Flags.Hidden | Building.Flags.EventActive |
                                Building.Flags.Flooded | Building.Flags.Filling | Building.Flags.Historical;
        }

        public override void SetFilling(ushort buildingID, ref Building data, bool filling)
        {
            if (filling)
                data.m_flags = data.m_flags & (Building.Flags.ContentMask | Building.Flags.IncomingOutgoing |
                                               Building.Flags.CapacityFull | Building.Flags.Created |
                                               Building.Flags.Deleted | Building.Flags.Original |
                                               Building.Flags.CustomName | Building.Flags.Untouchable |
                                               Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                               Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                               Building.Flags.Evacuating | Building.Flags.Completed |
                                               Building.Flags.Active | Building.Flags.Abandoned |
                                               Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                               Building.Flags.Collapsed | Building.Flags.Upgrading |
                                               Building.Flags.SecondaryLoading | Building.Flags.Hidden |
                                               Building.Flags.EventActive | Building.Flags.Flooded |
                                               Building.Flags.Filling | Building.Flags.Historical) |
                               Building.Flags.Filling;
            else
                data.m_flags &= Building.Flags.ContentMask | Building.Flags.IncomingOutgoing |
                                Building.Flags.CapacityFull | Building.Flags.Created | Building.Flags.Deleted |
                                Building.Flags.Original | Building.Flags.CustomName | Building.Flags.Untouchable |
                                Building.Flags.FixedHeight | Building.Flags.RateReduced | Building.Flags.HighDensity |
                                Building.Flags.RoadAccessFailed | Building.Flags.Evacuating | Building.Flags.Completed |
                                Building.Flags.Active | Building.Flags.Abandoned | Building.Flags.Demolishing |
                                Building.Flags.ZonesUpdated | Building.Flags.Downgrading | Building.Flags.Collapsed |
                                Building.Flags.Upgrading | Building.Flags.SecondaryLoading | Building.Flags.Hidden |
                                Building.Flags.EventActive | Building.Flags.Flooded | Building.Flags.Historical;
        }

        public void SetTransferReason(
            ushort buildingID,
            ref Building data,
            TransferManager.TransferReason material)
        {
            if (this.m_storageType != TransferManager.TransferReason.None)
                return;
            TransferManager.TransferReason seniors = (TransferManager.TransferReason) data.m_seniors;
            if (material != seniors)
            {
                if (seniors != TransferManager.TransferReason.None)
                {
                    Singleton<TransferManager>.instance.RemoveIncomingOffer(seniors, new TransferManager.TransferOffer()
                    {
                        Building = buildingID
                    });
                    this.CancelIncomingTransfer(buildingID, ref data, seniors);
                }

                data.m_seniors = (byte) material;
                if (data.m_customBuffer1 == (ushort) 0)
                {
                    data.m_adults = (byte) material;
                    this.SetContentFlags(buildingID, ref data, material);
                }
            }

            Notification.Problem problems = data.m_problems;
            data.m_problems = material != TransferManager.TransferReason.None
                ? Notification.RemoveProblems(data.m_problems, Notification.Problem.ResourceNotSelected)
                : Notification.AddProblems(data.m_problems, Notification.Problem.ResourceNotSelected);
            if (data.m_problems == problems)
                return;
            Singleton<BuildingManager>.instance.UpdateNotifications(buildingID, problems, data.m_problems);
        }

        private void SetContentFlags(
            ushort buildingID,
            ref Building data,
            TransferManager.TransferReason material)
        {
            switch (material)
            {
                case TransferManager.TransferReason.Goods:
                    data.m_flags = data.m_flags & (Building.Flags.Content07 | Building.Flags.IncomingOutgoing |
                                                   Building.Flags.CapacityFull | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content07;
                    break;
                case TransferManager.TransferReason.Coal:
                    data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                                   Building.Flags.Created | Building.Flags.Deleted |
                                                   Building.Flags.Original | Building.Flags.CustomName |
                                                   Building.Flags.Untouchable | Building.Flags.FixedHeight |
                                                   Building.Flags.RateReduced | Building.Flags.HighDensity |
                                                   Building.Flags.LevelUpLandValue | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.LevelUpLandValue;
                    break;
                case TransferManager.TransferReason.AnimalProducts:
                    data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                                   Building.Flags.Created | Building.Flags.Deleted |
                                                   Building.Flags.Original | Building.Flags.CustomName |
                                                   Building.Flags.Untouchable | Building.Flags.FixedHeight |
                                                   Building.Flags.RateReduced | Building.Flags.HighDensity |
                                                   Building.Flags.LevelUpEducation | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.LevelUpEducation;
                    break;
                case TransferManager.TransferReason.Flours:
                    data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                                   Building.Flags.Content03 | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content03;
                    break;
                case TransferManager.TransferReason.Paper:
                    data.m_flags = data.m_flags & (Building.Flags.Content04_Forbid | Building.Flags.IncomingOutgoing |
                                                   Building.Flags.CapacityFull | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content04_Forbid;
                    break;
                case TransferManager.TransferReason.PlanedTimber:
                    data.m_flags = data.m_flags & (Building.Flags.Content01_Forbid | Building.Flags.IncomingOutgoing |
                                                   Building.Flags.CapacityFull | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content01_Forbid;
                    break;
                case TransferManager.TransferReason.Petroleum:
                    data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                                   Building.Flags.Content05 | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content05;
                    break;
                case TransferManager.TransferReason.Plastics:
                    data.m_flags = data.m_flags & (Building.Flags.Content02_Forbid | Building.Flags.IncomingOutgoing |
                                                   Building.Flags.CapacityFull | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content02_Forbid;
                    break;
                case TransferManager.TransferReason.Glass:
                    data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                                   Building.Flags.Content06 | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content06;
                    break;
                case TransferManager.TransferReason.Metals:
                    data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                                   Building.Flags.Content05_Forbid | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content05_Forbid;
                    break;
                case TransferManager.TransferReason.LuxuryProducts:
                    data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                                   Building.Flags.Content06_Forbid | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content06_Forbid;
                    break;
                default:
                    if (material != TransferManager.TransferReason.Petrol)
                    {
                        if (material != TransferManager.TransferReason.Food)
                        {
                            if (material == TransferManager.TransferReason.Lumber)
                            {
                                data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing |
                                                               Building.Flags.CapacityFull | Building.Flags.Created |
                                                               Building.Flags.Deleted | Building.Flags.Original |
                                                               Building.Flags.CustomName | Building.Flags.Untouchable |
                                                               Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                               Building.Flags.HighDensity |
                                                               Building.Flags.RoadAccessFailed |
                                                               Building.Flags.Evacuating | Building.Flags.Completed |
                                                               Building.Flags.Active | Building.Flags.Abandoned |
                                                               Building.Flags.Demolishing |
                                                               Building.Flags.ZonesUpdated |
                                                               Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                               Building.Flags.Upgrading | Building.Flags.Loading2 |
                                                               Building.Flags.SecondaryLoading | Building.Flags.Hidden |
                                                               Building.Flags.EventActive | Building.Flags.Flooded |
                                                               Building.Flags.Filling | Building.Flags.Historical) |
                                               Building.Flags.Loading2;
                                break;
                            }

                            data.m_flags &= Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                            Building.Flags.Created | Building.Flags.Deleted | Building.Flags.Original |
                                            Building.Flags.CustomName | Building.Flags.Untouchable |
                                            Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                            Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                            Building.Flags.Evacuating | Building.Flags.Completed |
                                            Building.Flags.Active | Building.Flags.Abandoned |
                                            Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                            Building.Flags.Downgrading | Building.Flags.Collapsed |
                                            Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                            Building.Flags.Hidden | Building.Flags.EventActive |
                                            Building.Flags.Flooded | Building.Flags.Filling | Building.Flags.Historical;
                            break;
                        }

                        data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                                       Building.Flags.Created | Building.Flags.Deleted |
                                                       Building.Flags.Original | Building.Flags.CustomName |
                                                       Building.Flags.Untouchable | Building.Flags.FixedHeight |
                                                       Building.Flags.RateReduced | Building.Flags.HighDensity |
                                                       Building.Flags.RoadAccessFailed | Building.Flags.Evacuating |
                                                       Building.Flags.Completed | Building.Flags.Active |
                                                       Building.Flags.Abandoned | Building.Flags.Demolishing |
                                                       Building.Flags.ZonesUpdated | Building.Flags.Downgrading |
                                                       Building.Flags.Collapsed | Building.Flags.Upgrading |
                                                       Building.Flags.Loading1 | Building.Flags.SecondaryLoading |
                                                       Building.Flags.Hidden | Building.Flags.EventActive |
                                                       Building.Flags.Flooded | Building.Flags.Filling |
                                                       Building.Flags.Historical) | Building.Flags.Loading1;
                        break;
                    }

                    data.m_flags = data.m_flags & (Building.Flags.IncomingOutgoing | Building.Flags.CapacityFull |
                                                   Building.Flags.Content03_Forbid | Building.Flags.Created |
                                                   Building.Flags.Deleted | Building.Flags.Original |
                                                   Building.Flags.CustomName | Building.Flags.Untouchable |
                                                   Building.Flags.FixedHeight | Building.Flags.RateReduced |
                                                   Building.Flags.HighDensity | Building.Flags.RoadAccessFailed |
                                                   Building.Flags.Evacuating | Building.Flags.Completed |
                                                   Building.Flags.Active | Building.Flags.Abandoned |
                                                   Building.Flags.Demolishing | Building.Flags.ZonesUpdated |
                                                   Building.Flags.Downgrading | Building.Flags.Collapsed |
                                                   Building.Flags.Upgrading | Building.Flags.SecondaryLoading |
                                                   Building.Flags.Hidden | Building.Flags.EventActive |
                                                   Building.Flags.Flooded | Building.Flags.Filling |
                                                   Building.Flags.Historical) | Building.Flags.Content03_Forbid;
                    break;
            }
        }

        private void CancelIncomingTransfer(
            ushort buildingID,
            ref Building data,
            TransferManager.TransferReason material)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort vehicleID = data.m_guestVehicles;
            int num = 0;
            while (vehicleID != (ushort) 0)
            {
                ushort nextGuestVehicle = instance.m_vehicles.m_buffer[(int) vehicleID].m_nextGuestVehicle;
                if ((TransferManager.TransferReason) instance.m_vehicles.m_buffer[(int) vehicleID].m_transferType ==
                    material &&
                    (instance.m_vehicles.m_buffer[(int) vehicleID].m_flags &
                     (Vehicle.Flags.TransferToTarget | Vehicle.Flags.GoingBack)) == Vehicle.Flags.TransferToTarget &&
                    (int) instance.m_vehicles.m_buffer[(int) vehicleID].m_targetBuilding == (int) buildingID)
                    instance.m_vehicles.m_buffer[(int) vehicleID].Info.m_vehicleAI.SetTarget(vehicleID,
                        ref instance.m_vehicles.m_buffer[(int) vehicleID], (ushort) 0);
                vehicleID = nextGuestVehicle;
                if (++num > CargoFerriesMod.MaxVehicleCount)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core,
                        "Invalid list detected!\n" + System.Environment.StackTrace);
                    break;
                }
            }
        }


        public override bool IsFull(ushort buildingID, ref Building data) =>
            (int) data.m_customBuffer1 * 100 >= this.m_storageCapacity;

        public override bool CanBeRelocated(ushort buildingID, ref Building data) =>
            (int) data.m_customBuffer1 * 100 == 0;

        public TransferManager.TransferReason GetTransferReason(
            ushort buildingID,
            ref Building data)
        {
            return this.m_storageType != TransferManager.TransferReason.None
                ? this.m_storageType
                : (TransferManager.TransferReason) data.m_seniors;
        }

        public TransferManager.TransferReason GetActualTransferReason(
            ushort buildingID,
            ref Building data)
        {
            return this.m_storageType != TransferManager.TransferReason.None
                ? this.m_storageType
                : (TransferManager.TransferReason) data.m_adults;
        }

        private int GetMaxLoadSize() => 8000;

        public override string GetLocalizedTooltip()
        {
            string str1 =
                LocaleFormatter.FormatGeneric("AIINFO_WATER_CONSUMPTION", (object) (this.GetWaterConsumption() * 16)) +
                System.Environment.NewLine + LocaleFormatter.FormatGeneric("AIINFO_ELECTRICITY_CONSUMPTION",
                    (object) (this.GetElectricityConsumption() * 16));
            string str2 = LocaleFormatter.FormatGeneric("AIINFO_CAPACITY", (object) this.m_storageCapacity) +
                          System.Environment.NewLine + LocaleFormatter.FormatGeneric("AIINFO_INDUSTRY_VEHICLE_COUNT",
                              (object) this.m_truckCount);
            string baseTooltip = TooltipHelper.Append(base.GetLocalizedTooltip(),
                TooltipHelper.Format(LocaleFormatter.Info1, str1, LocaleFormatter.Info2, str2));
            string addTooltip1 = TooltipHelper.Format("arrowVisible", "false", "input1Visible", "true", "input2Visible",
                "false", "input3Visible", "false", "input4Visible", "false", "outputVisible", "false");
            string addTooltip2 = TooltipHelper.Format("input1",
                IndustryWorldInfoPanel.ResourceSpriteName(this.m_storageType, true), "input2", string.Empty, "input3",
                string.Empty, "input4", string.Empty, "output", string.Empty);
            return TooltipHelper.Append(TooltipHelper.Append(baseTooltip, addTooltip1), addTooltip2);
        }

        public override string GetLocalizedStats(ushort buildingID, ref Building data)
        {
            string str = string.Empty;
            TransferManager.TransferReason actualTransferReason = this.GetActualTransferReason(buildingID, ref data);
            if (actualTransferReason != TransferManager.TransferReason.None)
            {
                int num1 = (PlayerBuildingAI.GetProductionRate(100,
                    Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class)) * this.m_truckCount + 99) / 100;
                int count = 0;
                int cargo = 0;
                int capacity = 0;
                int outside = 0;
                this.CalculateOwnVehicles(buildingID, ref data, actualTransferReason, ref count, ref cargo,
                    ref capacity, ref outside);
                int num2 = (int) data.m_customBuffer1 * 100;
                int num3 = 0;
                if (num2 != 0)
                    num3 = Mathf.Max(1, num2 * 100 / this.m_storageCapacity);
                int num = (int) data.m_customBuffer1 * 100;
                str = str + StringUtils.SafeFormat(ColossalFramework.Globalization.Locale.Get("INDUSTRYPANEL_BUFFERTOOLTIP"), (object) IndustryWorldInfoPanel.FormatResource((ulong) (uint) num), (object) IndustryWorldInfoPanel.FormatResourceWithUnit((uint) m_storageCapacity, actualTransferReason))
                + " (" + LocaleFormatter.FormatGeneric("AIINFO_FULL", (object) num3) + ")" + System.Environment.NewLine +
                      LocaleFormatter.FormatGeneric("AIINFO_INDUSTRY_VEHICLES", (object) count, (object) num1);
            }

            return str;
        }

        //copied from CommonBuildingAI, except for the marked line
        protected new void CalculateOwnVehicles(
            ushort buildingID,
            ref Building data,
            TransferManager.TransferReason material,
            ref int count,
            ref int cargo,
            ref int capacity,
            ref int outside)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort vehicleID = data.m_ownVehicles;
            int num = 0;
            while (vehicleID != (ushort) 0)
            {
                //added check for tourist count to distinguish between really own vehicles and transit vehicles
                if ((TransferManager.TransferReason) instance.m_vehicles.m_buffer[(int) vehicleID].m_transferType ==
                    material && instance.m_vehicles.m_buffer[(int) vehicleID].m_touristCount == 1)
                {
                    int size;
                    int max;
                    instance.m_vehicles.m_buffer[(int) vehicleID].Info.m_vehicleAI.GetSize(vehicleID,
                        ref instance.m_vehicles.m_buffer[(int) vehicleID], out size, out max);
                    cargo += Mathf.Min(size, max);
                    capacity += max;
                    ++count;
                    if (
                        (instance.m_vehicles.m_buffer[(int) vehicleID].m_flags &
                         (Vehicle.Flags.Importing | Vehicle.Flags.Exporting)) != ~(Vehicle.Flags.Created |
                            Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
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
                        ++outside;
                }

                vehicleID = instance.m_vehicles.m_buffer[(int) vehicleID].m_nextOwnVehicle;
                if (++num > CargoFerriesMod.MaxVehicleCount)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core,
                        "Invalid list detected!\n" + System.Environment.StackTrace);
                    break;
                }
            }
        }
    }
}
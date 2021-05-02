using System;
using CargoFerries.AI;
using CargoFerries.Utils;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace CargoFerries
{
    public class GamePanelExtender : MonoBehaviour
    {
        private bool _initialized;
        private CityServiceWorldInfoPanel _cityServiceInfoPanel;
        private UIDropDown m_dropdownResource;
        private UIDropDown m_dropdownMode;
        protected InstanceID m_InstanceID;
        private ushort buildingId = 0;

        private TransferManager.TransferReason[] m_transferReasons = new TransferManager.TransferReason[16]
        {
            TransferManager.TransferReason.None,
            TransferManager.TransferReason.Fish,
            TransferManager.TransferReason.AnimalProducts,
            TransferManager.TransferReason.Flours,
            TransferManager.TransferReason.Paper,
            TransferManager.TransferReason.PlanedTimber,
            TransferManager.TransferReason.Petroleum,
            TransferManager.TransferReason.Plastics,
            TransferManager.TransferReason.Glass,
            TransferManager.TransferReason.Metals,
            TransferManager.TransferReason.LuxuryProducts,
            TransferManager.TransferReason.Lumber,
            TransferManager.TransferReason.Food,
            TransferManager.TransferReason.Coal,
            TransferManager.TransferReason.Petrol,
            TransferManager.TransferReason.Goods
        };

        private WarehouseMode[] m_warehouseModes = new WarehouseMode[3]
        {
            WarehouseMode.Balanced,
            WarehouseMode.Import,
            WarehouseMode.Export
        };

        private enum WarehouseMode
        {
            Balanced,
            Import,
            Export,
        }

        public void OnDestroy()
        {
            if (m_dropdownResource != null)
            {
                if (_cityServiceInfoPanel != null)
                {
                    _cityServiceInfoPanel.component.RemoveUIComponent(m_dropdownResource);
                }

                Destroy(m_dropdownResource.gameObject);
                m_dropdownResource = null;
            }

            if (m_dropdownMode != null)
            {
                if (_cityServiceInfoPanel != null)
                {
                    _cityServiceInfoPanel.component.RemoveUIComponent(m_dropdownMode);
                }

                Destroy(m_dropdownMode.gameObject);
                m_dropdownMode = null;
            }

            _initialized = false;
        }

        public void Update()
        {
            if (!_initialized)
            {
                var go = GameObject.Find("(Library) CityServiceWorldInfoPanel");
                if (go == null)
                {
                    return;
                }

                var infoPanel = go.GetComponent<CityServiceWorldInfoPanel>();
                if (infoPanel == null)
                {
                    return;
                }

                _cityServiceInfoPanel = infoPanel;
                m_dropdownResource = Utils.UIUtils.CreateDropDown(_cityServiceInfoPanel.component);
                m_dropdownResource.relativePosition = new Vector3(105, 267);
                m_dropdownResource.width = 250;
                m_dropdownMode = Utils.UIUtils.CreateDropDown(_cityServiceInfoPanel.component);
                m_dropdownMode.relativePosition = new Vector3(110 + m_dropdownResource.width, 267);
                m_dropdownMode.width = 120;
                RefreshDropdownLists();
                this.m_dropdownResource.eventSelectedIndexChanged += OnDropdownResourceChanged;
                this.m_dropdownMode.eventSelectedIndexChanged += OnDropdownModeChanged;
                _initialized = true;
            }

            if (!_cityServiceInfoPanel.component.isVisible)
            {
                return;
            }

            SetUpSwapButton();
        }

        private void SetUpSwapButton()
        {
            var instance = (InstanceID) Util.GetInstanceField(typeof(CityServiceWorldInfoPanel), _cityServiceInfoPanel,
                "m_InstanceID");
            var id = instance.Building;
            if (buildingId == id)
            {
                return;
            }

            buildingId = id;
            
            
            var data = Singleton<BuildingManager>.instance.m_buildings.m_buffer[id];
            var isCargoStation = CargoFerryHarborAI.IsCargoFerryHarbor(data);
            m_dropdownResource.isVisible = isCargoStation;
            m_dropdownMode.isVisible = isCargoStation;
            if (isCargoStation)
            {
                this.m_InstanceID = instance;
                var buildingAi = data.Info.m_buildingAI as CargoFerryWarehouseHarborAI;
                int num = 0;
                foreach (TransferManager.TransferReason transferReason in this.m_transferReasons)
                {
                    if (transferReason == buildingAi.GetTransferReason(this.m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building]))
                    {
                        this.m_dropdownResource.selectedIndex = num;
                        break;
                    }
                    ++num;
                }
                this.m_dropdownMode.selectedIndex = (int) this.warehouseMode;
                
            }
        }

        private void RefreshDropdownLists()
        {
            string[] strArray1 = new string[this.m_transferReasons.Length];
            for (int index = 0; index < this.m_transferReasons.Length; ++index)
            {
                string str = ColossalFramework.Globalization.Locale.Get("WAREHOUSEPANEL_RESOURCE",
                    this.m_transferReasons[index].ToString());
                strArray1[index] = str;
            }

            this.m_dropdownResource.items = strArray1;
            string[] strArray2 = new string[this.m_warehouseModes.Length];
            for (int index = 0; index < this.m_warehouseModes.Length; ++index)
            {
                string str = ColossalFramework.Globalization.Locale.Get("WAREHOUSEPANEL_MODE",
                    this.m_warehouseModes[index].ToString());
                strArray2[index] = str;
            }

            this.m_dropdownMode.items = strArray2;
        }

        private void OnDropdownModeChanged(UIComponent component, int index) =>
            this.warehouseMode = this.m_warehouseModes[index];

        private void OnDropdownResourceChanged(UIComponent component, int index)
        {
            CargoFerryWarehouseHarborAI ai = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building]
                .Info.m_buildingAI as CargoFerryWarehouseHarborAI;
            Singleton<SimulationManager>.instance.AddAction((System.Action) (() =>
                ai.SetTransferReason(this.m_InstanceID.Building,
                    ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building],
                    this.m_transferReasons[index])));
        }

        private WarehouseMode warehouseMode
        {
            get
            {
                if ((Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building]
                        .m_flags & Building.Flags.Filling) == Building.Flags.None &&
                    (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building]
                         .m_flags &
                     Building.Flags.Downgrading) == Building.Flags.None)
                    return WarehouseMode.Balanced;
                return (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building]
                    .m_flags & Building.Flags.Downgrading) != Building.Flags.None
                    ? WarehouseMode.Export
                    : WarehouseMode.Import;
            }
            set
            {
                switch (value)
                {
                    case WarehouseMode.Balanced:
                        Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building].Info
                            .m_buildingAI.SetEmptying(this.m_InstanceID.Building,
                                ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[
                                    (int) this.m_InstanceID.Building], false);
                        Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building].Info
                            .m_buildingAI.SetFilling(this.m_InstanceID.Building,
                                ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[
                                    (int) this.m_InstanceID.Building], false);
                        break;
                    case WarehouseMode.Import:
                        Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building].Info
                            .m_buildingAI.SetEmptying(this.m_InstanceID.Building,
                                ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[
                                    (int) this.m_InstanceID.Building], false);
                        Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building].Info
                            .m_buildingAI.SetFilling(this.m_InstanceID.Building,
                                ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[
                                    (int) this.m_InstanceID.Building], true);
                        break;
                    case WarehouseMode.Export:
                        Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building].Info
                            .m_buildingAI.SetEmptying(this.m_InstanceID.Building,
                                ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[
                                    (int) this.m_InstanceID.Building], true);
                        Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) this.m_InstanceID.Building].Info
                            .m_buildingAI.SetFilling(this.m_InstanceID.Building,
                                ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[
                                    (int) this.m_InstanceID.Building], false);
                        break;
                }
            }
        }
    }
}
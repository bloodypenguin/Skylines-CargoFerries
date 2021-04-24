using System;
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

namespace CargoFerries.AI
{
    
    //based of CargoHarborAI but without animals, connections & checking height
    public class CargoFerryHarborAI : CargoStationAI
    {
        [NonSerialized] protected float m_quayOffset;

        public override void InitializePrefab()
        {
            base.InitializePrefab();
            float a = this.m_info.m_generatedInfo.m_max.z - 7f;
            if (this.m_info.m_paths != null)
            {
                for (int index1 = 0; index1 < this.m_info.m_paths.Length; ++index1)
                {
                    if (this.m_info.m_paths[index1].m_netInfo != null &&
                        this.m_info.m_paths[index1].m_netInfo.m_class.m_service == ItemClass.Service.Road &&
                        this.m_info.m_paths[index1].m_nodes != null)
                    {
                        for (int index2 = 0; index2 < this.m_info.m_paths[index1].m_nodes.Length; ++index2)
                            a = Mathf.Min(a,
                                -16f - this.m_info.m_paths[index1].m_netInfo.m_halfWidth -
                                this.m_info.m_paths[index1].m_nodes[index2].z);
                    }
                }
            }

            this.m_quayOffset = a;
        }

        public override ToolBase.ToolErrors CheckBuildPosition(
            ushort relocateID,
            ref Vector3 position,
            ref float angle,
            float waterHeight,
            float elevation,
            ref Segment3 connectionSegment,
            out int productionRate,
            out int constructionCost)
        {
            ToolBase.ToolErrors toolErrors1 = ToolBase.ToolErrors.None;
            Vector3 pos;
            Vector3 dir;
            bool isQuay;
            if (this.m_info.m_placementMode == BuildingInfo.PlacementMode.Shoreline &&
                BuildingTool.SnapToCanal(position, out pos, out dir, out isQuay, 40f, false))
            {
                angle = Mathf.Atan2(dir.x, -dir.z);
                pos += dir * this.m_quayOffset;
                position.x = pos.x;
                position.z = pos.z;
                if (!isQuay)
                    toolErrors1 |= ToolBase.ToolErrors.ShoreNotFound;
            }

            ToolBase.ToolErrors toolErrors2 = toolErrors1 | base.CheckBuildPosition(relocateID, ref position, ref angle,
                waterHeight, elevation, ref connectionSegment, out productionRate, out constructionCost);

            return toolErrors2;
        }

        public override bool GetWaterStructureCollisionRange(out float min, out float max)
        {
            if (this.m_info.m_placementMode != BuildingInfo.PlacementMode.Shoreline)
                return base.GetWaterStructureCollisionRange(out min, out max);
            min = 20f / Mathf.Max(22f, (float) this.m_info.m_cellLength * 8f);
            max = 1f;
            return true;
        }

        public override bool RequireRoadAccess() => true;
    }
}
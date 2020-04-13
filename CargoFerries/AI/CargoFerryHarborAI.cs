using ColossalFramework.Math;
using UnityEngine;

namespace CargoFerries.AI
{
    public class CargoFerryHarborAI : CargoHarborAI
    {
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
            var errors = base.CheckBuildPosition(relocateID, ref position, ref angle, waterHeight, elevation,
                ref connectionSegment, out productionRate, out constructionCost);
            return errors & ~ToolBase.ToolErrors.CannotConnect;
        }
    }
}
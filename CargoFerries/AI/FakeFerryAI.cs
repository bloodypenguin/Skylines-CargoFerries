using System.Collections.Generic;
using UnityEngine;

namespace CargoFerries.AI
{
    //TODO: handle concurrency?
    //TODO: clean up fake AIs
    public class FakeFerryAI : FerryAI
    {
        private static Dictionary<string, FakeFerryAI> _fakeAIs = new Dictionary<string, FakeFerryAI>();

        public new void CalculateSegmentPosition(
            ushort vehicleID,
            ref Vehicle vehicleData,
            PathUnit.Position position,
            uint laneID,
            byte offset,
            out Vector3 pos,
            out Vector3 dir,
            out float maxSpeed)
        {
            base.CalculateSegmentPosition(vehicleID, ref vehicleData, position, laneID, offset, out pos, out dir, out maxSpeed);
        }
        
        public new void CalculateSegmentPosition(
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
            base.CalculateSegmentPosition(vehicleID, ref vehicleData, nextPosition, position, laneID, offset,
                    prevPos, prevLaneID, prevOffset, index,
                    out pos, out dir, out maxSpeed);
        }
        
        public new bool StartPathFind(
            ushort vehicleID,
            ref Vehicle vehicleData,
            Vector3 startPos,
            Vector3 endPos)
        {
            return base.StartPathFind(vehicleID, ref vehicleData, startPos, endPos);
        }

        public static FakeFerryAI GetFakeShipAI(CargoFerryAI ferryAi)
        {
            if (_fakeAIs.ContainsKey(ferryAi.m_info.name))
            {
                return _fakeAIs[ferryAi.m_info.name];
            }
            var ai = new FakeFerryAI()
            {
                m_info = ferryAi.m_info,
            };
            _fakeAIs[ferryAi.m_info.name] = ai;
            return _fakeAIs[ferryAi.m_info.name];
        }
    }
}
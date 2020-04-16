using System.Collections.Generic;

namespace CargoFerries.AI
{
    //TODO: handle concurrency?
    //TODO: clean up fake AIs
    public class FakeCargoShipAI : CargoShipAI
    {
        private static Dictionary<string, FakeCargoShipAI> _fakeAIs = new Dictionary<string, FakeCargoShipAI>();

        public bool StartPathFind1(ushort vehicleID, ref Vehicle vehicleData)
        {
            return this.StartPathFind(vehicleID, ref vehicleData);
        }

        public static FakeCargoShipAI GetFakeShipAI(CargoFerryAI ferryAi)
        {
            if (_fakeAIs.ContainsKey(ferryAi.m_info.name))
            {
                return _fakeAIs[ferryAi.m_info.name];
            }
            var ai = new FakeCargoShipAI()
            {
                m_info = ferryAi.m_info,
                m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry"),
                m_cargoCapacity = ferryAi.m_cargoCapacity
            };
            _fakeAIs[ferryAi.m_info.name] = ai;
            return _fakeAIs[ferryAi.m_info.name];
        }
    }
}
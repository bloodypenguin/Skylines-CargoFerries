using UnityEngine;

namespace CargoFerries
{
    public static class ItemClasses
    {
        public static ItemClass cargoFerryFacility = CreateItemClass("Ferry Cargo Facility");
        public static ItemClass cargoFerryVehicle = CreateItemClass("Ferry Cargo Vehicle");
        
        private static ItemClass CreateItemClass(string name)
        {
            var createInstance = ScriptableObject.CreateInstance<ItemClass>();
            createInstance.name = name;
            createInstance.m_level = ItemClass.Level.Level3;
            createInstance.m_service = ItemClass.Service.PublicTransport;
            createInstance.m_subService = ItemClass.SubService.PublicTransportShip;
            return createInstance;
        }
    }
}
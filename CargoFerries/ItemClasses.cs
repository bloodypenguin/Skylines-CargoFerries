using UnityEngine;

namespace CargoFerries
{
    public static class ItemClasses
    {
        public static ItemClass cargoFerryFacility = CreateFerryItemClass("Ferry Cargo Facility");
        public static ItemClass cargoFerryVehicle = CreateFerryItemClass("Ferry Cargo Vehicle");
        public static ItemClass cargoHelicopterFacility = CreateFerryItemClass("Helicopter Cargo Facility");
        public static ItemClass cargoHelicopterVehicle = CreateFerryItemClass("HelicopterFerry Cargo Vehicle");
        
        private static ItemClass CreateFerryItemClass(string name)
        {
            var createInstance = ScriptableObject.CreateInstance<ItemClass>();
            createInstance.name = name;
            createInstance.m_level = ItemClass.Level.Level5;
            createInstance.m_service = ItemClass.Service.PublicTransport;
            createInstance.m_subService = ItemClass.SubService.PublicTransportShip;
            return createInstance;
        }
        
        private static ItemClass CreateHelicopterItemClass(string name)
        {
            var createInstance = ScriptableObject.CreateInstance<ItemClass>();
            createInstance.name = name;
            createInstance.m_level = ItemClass.Level.Level5;
            createInstance.m_service = ItemClass.Service.PublicTransport;
            createInstance.m_subService = ItemClass.SubService.PublicTransportPlane;
            return createInstance;
        }
    }
}
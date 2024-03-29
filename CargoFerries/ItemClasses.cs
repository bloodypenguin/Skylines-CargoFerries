using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CargoFerries
{
    public static class ItemClasses
    {
        public static readonly ItemClass cargoFerryFacility = CreateFerryItemClass("Ferry Cargo Facility");
        public static readonly ItemClass cargoFerryVehicle = CreateFerryItemClass("Ferry Cargo Vehicle");

        public static void Register()
        {
            var dictionary = ((Dictionary<string, ItemClass>)typeof(ItemClassCollection).GetField("m_classDict", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
            if (!dictionary.ContainsKey(ItemClasses.cargoFerryFacility.name))
            {
                dictionary.Add(ItemClasses.cargoFerryFacility.name, ItemClasses.cargoFerryFacility);
            }
            if (!dictionary.ContainsKey(ItemClasses.cargoFerryVehicle.name))
            {
                dictionary.Add(ItemClasses.cargoFerryVehicle.name, ItemClasses.cargoFerryVehicle);
            }
        }

        public static void Unregister()
        {
            var dictionary = ((Dictionary<string, ItemClass>)typeof(ItemClassCollection).GetField("m_classDict", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
            dictionary.Remove(ItemClasses.cargoFerryFacility.name);
            dictionary.Remove(ItemClasses.cargoFerryVehicle.name);
        }
        
        private static ItemClass CreateFerryItemClass(string name)
        {
            var createInstance = ScriptableObject.CreateInstance<ItemClass>();
            createInstance.name = name;
            createInstance.m_level = ItemClass.Level.Level5;
            createInstance.m_service = ItemClass.Service.PublicTransport;
            createInstance.m_subService = ItemClass.SubService.PublicTransportShip;
            return createInstance;
        }
    }
}
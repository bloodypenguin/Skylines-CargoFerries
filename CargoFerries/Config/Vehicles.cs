using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using CargoFerries.OptionsFramework;

namespace CargoFerries.Config
{
    public static class Vehicles
    {
        private static readonly Dictionary<VehicleCategory, VehicleItem[]> _ids = new Dictionary<VehicleCategory, VehicleItem[]>
        {
            {
                VehicleCategory.CargoShip, new[]
                {
                    new VehicleItem(859777509, "River Container Ship Pack 1"), 
                    new VehicleItem(862748181, "River Container Ship Pack 2"), 
                    new VehicleItem(863432439, "River Container Ship Pack 3"), 
                    new VehicleItem(933246365, "Small River Container Ship 1"), 
                    new VehicleItem(933247367, "Small River Container Ship 2"), 
                    new VehicleItem(933247999, "Small River Container Ship 3"),
                }
            },
            {
                VehicleCategory.CargoHelicopter, new VehicleItem[]
                {
                }
            }
        };

        private static bool _configIsOverriden;

        public static IEnumerable<VehicleItem> GetItems(VehicleCategory vehicleCategory = VehicleCategory.All)
        {
            var list = new List<VehicleItem>();
            _ids.Where(kvp => (kvp.Key & vehicleCategory) != 0).Select(kvp => kvp.Value).ForEach(a => list.AddRange(a));
            return list;
        }

        private static Dictionary<VehicleCategory, VehicleItem[]> Ids  {
            get
            {
                if (_configIsOverriden)
                {
                    return _ids;
                }
                _ids[VehicleCategory.CargoShip] = OptionsWrapper<Config>.Options.CargoFerries.Items.ToArray();
                _ids[VehicleCategory.CargoHelicopter] = OptionsWrapper<Config>.Options.CargoHelicopters.Items.ToArray();
                _configIsOverriden = true;
                return _ids;
            }
        }

        public static long[] GetConvertedIds(VehicleCategory vehicleCategory = VehicleCategory.All)
        {
            var list = new List<long>();
            Ids.Where(kvp => IsCategoryEnabled(kvp.Key) && (kvp.Key & vehicleCategory) != 0).Select(kvp => kvp.Value).ForEach(l => l.ForEach(t =>
            {
                if (!t.Exclude)
                {
                    list.Add(t.WorkshopId);
                }
            }));
            return list.ToArray();
        }

        private static bool IsCategoryEnabled(VehicleCategory vehicleCategory)
        {
            switch (vehicleCategory)
            {
                case VehicleCategory.CargoShip:
                    return Util.DLC(SteamHelper.kMotionDLCAppID) ;
                case VehicleCategory.CargoHelicopter:
                    return Util.DLC(SteamHelper.kUrbanDLCAppID) ;
                default:
                    throw new ArgumentOutOfRangeException(nameof(vehicleCategory), vehicleCategory, null);
            }
        }
    }
}
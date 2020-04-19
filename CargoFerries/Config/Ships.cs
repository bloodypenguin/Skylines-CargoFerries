using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using CargoFerries.OptionsFramework;

namespace CargoFerries.Config
{
    public static class Ships
    {
        private static readonly Dictionary<VehicleCategory, VehicleItem[]> _ids = new Dictionary<VehicleCategory, VehicleItem[]>
        {
            {
                VehicleCategory.CargoShip, new[]
                {
                    new VehicleItem(933246365, "Small River Container Ship 1"), 
                }
            },
            {
                VehicleCategory.CargoHelicopter, new[]
                {
                    new VehicleItem(2049590763, "mi-8 hip passenger helicopter"), 
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
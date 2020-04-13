using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using CargoFerries.OptionsFramework;

namespace CargoFerries.Config
{
    public static class Ships
    {
        private static readonly Dictionary<ShipCategory, ShipItem[]> _ids = new Dictionary<ShipCategory, ShipItem[]>
        {
            {
                ShipCategory.CargoShip, new[]
                {
                    new ShipItem(933246365, "Small River Container Ship 1"), 
                }
            },
        };

        private static bool _configIsOverriden;

        public static IEnumerable<ShipItem> GetItems(ShipCategory shipCategory = ShipCategory.All)
        {
            var list = new List<ShipItem>();
            _ids.Where(kvp => (kvp.Key & shipCategory) != 0).Select(kvp => kvp.Value).ForEach(a => list.AddRange(a));
            return list;
        }

        private static Dictionary<ShipCategory, ShipItem[]> Ids  {
            get
            {
                if (_configIsOverriden)
                {
                    return _ids;
                }
                _ids[ShipCategory.CargoShip] = OptionsWrapper<Config>.Options.CargoShips.Items.ToArray();
                _configIsOverriden = true;
                return _ids;
            }
        }

        public static long[] GetConvertedIds(ShipCategory shipCategory = ShipCategory.All)
        {
            var list = new List<long>();
            Ids.Where(kvp => IsCategoryEnabled(kvp.Key) && (kvp.Key & shipCategory) != 0).Select(kvp => kvp.Value).ForEach(l => l.ForEach(t =>
            {
                if (!t.Exclude)
                {
                    list.Add(t.WorkshopId);
                }
            }));
            return list.ToArray();
        }

        private static bool IsCategoryEnabled(ShipCategory shipCategory)
        {
            switch (shipCategory)
            {
                case ShipCategory.CargoShip:
                    return Util.DLC(SteamHelper.kMotionDLCAppID) ;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shipCategory), shipCategory, null);
            }
        }
    }
}
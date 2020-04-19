using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CargoFerries.Config
{
    [Serializable]
    public class VehicleItems
    {
        public VehicleItems()
        {
            this.Items = new List<VehicleItem>();
        }

        public VehicleItems(List<VehicleItem> items)
        {
            this.Items = items;
        }

        [XmlElement("items")]
        public List<VehicleItem> Items { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CargoFerries.Config
{
    [Serializable]
    public class ShipItems
    {
        public ShipItems()
        {
            this.Items = new List<ShipItem>();
        }

        public ShipItems(List<ShipItem> items)
        {
            this.Items = items;
        }

        [XmlElement("items")]
        public List<ShipItem> Items { get; private set; }
    }
}

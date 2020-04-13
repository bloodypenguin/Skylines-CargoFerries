using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CargoFerries.Config
{
    [Serializable]
    public class ShipItem
    {
        public ShipItem()
        {
            Exclude = false;
            WorkshopId = -1;
            Description = string.Empty;
        }

        public ShipItem(long workshoId, string description)
        {
            Exclude = false;
            Description = description;
            WorkshopId = workshoId;
        }

        [XmlAttribute("workshop-id")]
        public long WorkshopId { get; private set; }
        [XmlAttribute("description")]
        public string Description { get; private set; }
        [XmlAttribute("exclude"), DefaultValue(false)]
        public bool Exclude { get; private set; }
    }
}
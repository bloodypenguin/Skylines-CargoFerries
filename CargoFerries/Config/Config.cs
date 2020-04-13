using System.Linq;
using System.Xml.Serialization;
using CargoFerries.OptionsFramework.Attibutes;

namespace CargoFerries.Config
{
    [Options("CargoFerries-Config")]
    public class Config
    {
        public Config()
        {
            CargoShips = new ShipItems(Ships.GetItems(ShipCategory.CargoShip).OrderBy(i => i.WorkshopId).ToList());
        }

        [XmlElement("version")]
        public int Version { get; set; }
        [XmlElement("cargo-ships-to-cargo-ferries")]
        public ShipItems CargoShips { get; private set; }
    }
}

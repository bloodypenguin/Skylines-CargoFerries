using System.Linq;
using System.Xml.Serialization;
using CargoFerries.OptionsFramework.Attibutes;

namespace CargoFerries.Config
{
    [Options("MoreCargoModes-Config")]
    public class Config
    {
        public Config()
        {
            CargoFerries = new VehicleItems(Vehicles.GetItems(VehicleCategory.CargoShip).OrderBy(i => i.WorkshopId).ToList());
            CargoHelicopters = new VehicleItems(Vehicles.GetItems(VehicleCategory.CargoHelicopter).OrderBy(i => i.WorkshopId).ToList());
        }

        [XmlElement("version")]
        public int Version { get; set; }
        [XmlElement("cargo-ships-to-cargo-ferries")]
        public VehicleItems CargoFerries { get; private set; }
        [XmlElement("helicopters-to-cargo-helicopters")]
        public VehicleItems CargoHelicopters { get; private set; }
    }
}

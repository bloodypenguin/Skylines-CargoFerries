using System.Linq;
using System.Xml.Serialization;
using CargoFerries.OptionsFramework.Attibutes;

namespace CargoFerries.Config
{
    [Options("Barges-Config")]
    public class Config
    {
        public Config()
        {
            Barges = new VehicleItems(Vehicles.GetItems(VehicleCategory.CargoShip).OrderBy(i => i.WorkshopId).ToList());
        }

        [XmlElement("version")]
        public int Version { get; set; }
        [XmlElement("cargo-ships-to-barges")]
        public VehicleItems Barges { get; private set; }
    }
}

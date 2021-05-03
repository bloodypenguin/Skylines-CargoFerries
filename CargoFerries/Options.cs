using System.Xml.Serialization;
using CargoFerries.Attributes;
using CargoFerries.OptionsFramework.Attibutes;

namespace CargoFerries
{
    [Options("Barges-Options")]
    public class Options
    {
        [HideInGameOrEditorCondition]
        [Checkbox("Built-in warehouse for barge harbors (Industries DLC is required)")]
        public bool EnableWarehouseAI { get; set; } = true;
        
        [HideWhenNotCargoHarborAI]
        [XmlIgnore]
        [Button("To barge harbor", null, nameof(EditedAssetTransformer), nameof(EditedAssetTransformer.ToBargeHarbor))]
        public object ToBargeHarborButton { get; set; } = null;
        
        [HideWhenNotCargoShipAI]
        [XmlIgnore]
        [Button("To barge", null, nameof(EditedAssetTransformer), nameof(EditedAssetTransformer.ToBarge))]
        public object ToBargeButton { get; set; } = null;
    }
}
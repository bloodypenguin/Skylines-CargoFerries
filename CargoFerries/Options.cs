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
        
        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To barge harbor", null, 
            nameof(CargoFerriesEditedAssetTransformer), nameof(CargoFerriesEditedAssetTransformer.ToBargeHarbor))]
        public object ToBargeHarborButton { get; set; } = null;
        
        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To barge", null, 
            nameof(CargoFerriesEditedAssetTransformer), nameof(CargoFerriesEditedAssetTransformer.ToBarge))]
        public object ToBargeButton { get; set; } = null;
    }
}
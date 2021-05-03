using System.Xml.Serialization;
using CargoFerries.OptionsFramework.Attibutes;

namespace CargoFerries
{
    [Options("Barges-Options")]
    public class Options
    {
        [HideInGameOrEditorCondition]
        [Checkbox("Built-in warehouse for barge harbors (Industries DLC is required)")]
        public bool EnableWarehouseAI { get; set; } = true;
    }
}
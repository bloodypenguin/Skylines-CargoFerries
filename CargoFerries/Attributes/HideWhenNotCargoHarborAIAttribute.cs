using System;
using CargoFerries.OptionsFramework.Attibutes;

namespace CargoFerries.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HideWhenNotCargoHarborAIAttribute : HideConditionAttribute
    {
        public override bool IsHidden()
        {
            return ToolsModifierControl.toolController?.m_editPrefabInfo is not BuildingInfo {m_buildingAI: CargoHarborAI _};
        }
    }
}
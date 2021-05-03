using System;
using CargoFerries.OptionsFramework.Attibutes;

namespace CargoFerries.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HideWhenNotCargoShipAIAttribute : HideConditionAttribute
    {
        public override bool IsHidden()
        {
            return ToolsModifierControl.toolController?.m_editPrefabInfo is not VehicleInfo {m_vehicleAI: CargoShipAI _};
        }
    }
}
using System;
using CargoFerries.OptionsFramework.Attibutes;

namespace CargoFerries.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HideInGameOrEditorConditionAttribute : HideConditionAttribute
    {
        public override bool IsHidden()
        {
            return SimulationManager.exists && SimulationManager.instance.m_metaData != null &&
                   SimulationManager.instance.m_metaData.m_updateMode != SimulationManager.UpdateMode.Undefined;
        }
    }
}
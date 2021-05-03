using System;

namespace CargoFerries.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ButtonAttribute : AbstractOptionsAttribute
    {
        public ButtonAttribute(string description, string group, string actionClass = null, string actionMethod = null) :
            base(description, group, actionClass, actionMethod)
        {

        }
    }
}
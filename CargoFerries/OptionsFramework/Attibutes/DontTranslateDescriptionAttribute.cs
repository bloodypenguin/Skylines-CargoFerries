using System;
using System.ComponentModel;

namespace CargoFerries.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.All)]
    public class DontTranslateDescriptionAttribute : DescriptionAttribute
    {
        public DontTranslateDescriptionAttribute(string description) :
            base(description)
        {
            
        }
    }
}
using System;
using ColossalFramework.UI;
using ICities;
using UnityEngine;
using CargoFerries.OptionsFramework;
using CargoFerries.OptionsFramework.Extensions;

namespace CargoFerries
{
    public class CargoFerriesMod : IUserMod
    {
        public static int MaxVehicleCount;
        
        public string Name => "Barges";
        public string Description => "Adds a new type of cargo transport - Barges. They are like cargo ships but use ferry paths & canals";

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>();
            try
            {
                OptionsWrapper<Config.Config>.Ensure();
            }
            catch (Exception e)
            {
                var display = new GameObject().AddComponent<ErrorMessageDisplay>();
                display.e = e;
            }
        }


        private class ErrorMessageDisplay : MonoBehaviour
        {
            public Exception e;

            public void Update()
            {
                var exceptionPanel = UIView.library?.ShowModal<ExceptionPanel>("ExceptionPanel");
                if (exceptionPanel == null)
                {
                    return;
                }
                exceptionPanel.SetMessage(
                "Malformed XML config",
                "There was an error reading Barges XML config:\n" + e.Message + "\n\nFalling back to default config...",
                true);
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}

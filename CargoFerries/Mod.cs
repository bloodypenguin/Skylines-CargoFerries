using System;
using ColossalFramework.UI;
using ICities;
using UnityEngine;
using CargoFerries.OptionsFramework;
using CargoFerries.OptionsFramework.Extensions;

namespace CargoFerries
{
    public class Mod : IUserMod
    {
        public string Name => "Cargo Ferries";
        public string Description => "Adds new type of cargo transport - cargo ferries. They are like cargo ships but use ferry paths & canals";

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
                "There was an error reading Cargo Ferries XML config:\n" + e.Message + "\n\nFalling back to default config...",
                true);
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}

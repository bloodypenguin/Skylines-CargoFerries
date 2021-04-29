using System;
using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;

namespace CargoFerries.Utils
{
    public static class Util
    {
        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                           | BindingFlags.Static;
            var field = type.GetField(fieldName, bindFlags);
            if (field == null)
            {
                throw new Exception(string.Format("Type '{0}' doesn't have field '{1}", type, fieldName));
            }
            return field.GetValue(instance);
        }
        
        public static bool IsModActive(string modNamePart)
        {
            try
            {
                var plugins = PluginManager.instance.GetPluginsInfo();
                return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.GetInstances<IUserMod>()
                    into instances
                    where instances.Any()
                    select instances[0].Name
                    into name
                    where name != null && name.Contains(modNamePart)
                    select name).Any();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to detect if mod with name containing {modNamePart} is active");
                Debug.LogException(e);
                return false;
            }
        }

        public static bool IsModActive(ulong modId)
        {
            try
            {
                var plugins = PluginManager.instance.GetPluginsInfo();
                return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.publishedFileID
                    into workshopId
                    where workshopId.AsUInt64 == modId
                    select workshopId).Any();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to detect if mod {modId} is active");
                Debug.LogException(e);
                return false;
            }
        }
    }
}
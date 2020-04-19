using System;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace CargoFerries.Utils
{
    public static class PatchUtil
    {
        private const string HarmonyId = "github.com/bloodypenguin/Skylines-CargoFerries";
        private static HarmonyInstance _harmonyInstance = null;

        private static HarmonyInstance HarmonyInstance =>
            _harmonyInstance ?? (_harmonyInstance = HarmonyInstance.Create(HarmonyId));

        public static void Patch(
            MethodDefinition original,
            MethodDefinition prefix = null,
            MethodDefinition postfix = null,
            MethodDefinition transpiler = null)
        {
            if (prefix == null && postfix == null && transpiler == null)
            {
                throw new Exception(
                    $"MCM: prefix, postfix and transpiler are null for method {original.Type.FullName}.{original.MethodName}");
            }

            try
            {
                Debug.Log($"MCM: Patching method {original.Type.FullName}.{original.MethodName}");
                var methodInfo = GetOriginal(original);
                HarmonyInstance.Patch(methodInfo,
                    prefix: prefix == null ? null : new HarmonyMethod(GetPatch(prefix)),
                    postfix: postfix == null ? null : new HarmonyMethod(GetPatch(postfix)),
                    transpiler == null ? null : new HarmonyMethod(GetPatch(transpiler))
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"MCM: Failed to patch method {original.Type.FullName}.{original.MethodName}");
                Debug.LogException(e);
            }
        }

        public static void Unpatch(MethodDefinition original)
        {
            Debug.Log($"MCM: Unpatching method {original.Type.FullName}.{original.MethodName}");
            HarmonyInstance.Unpatch(GetOriginal(original), HarmonyPatchType.All, HarmonyId);
        }

        private static MethodInfo GetOriginal(MethodDefinition original)
        {
            var bindingFlags = original.BindingFlags == BindingFlags.Default
                ? BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly
                : original.BindingFlags;
            var methodInfo = original.ArgumentTypes == null
                ? original.Type.GetMethod(original.MethodName, bindingFlags)
                : original.Type.GetMethod(original.MethodName, bindingFlags, null, original.ArgumentTypes, null);
            if (methodInfo == null)
            {
                throw new Exception(
                    $"MCM: Failed to find original method {original.Type.FullName}.{original.MethodName}");
            }

            return methodInfo;
        }

        private static MethodInfo GetPatch(MethodDefinition patch)
        {
            var bindingFlags = patch.BindingFlags == BindingFlags.Default
                ? BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly
                : patch.BindingFlags;
            var methodInfo = patch.ArgumentTypes == null
                ? patch.Type.GetMethod(patch.MethodName, bindingFlags)
                : patch.Type.GetMethod(patch.MethodName, bindingFlags, null, patch.ArgumentTypes, null);
            
            if (methodInfo == null)
            {
                throw new Exception($"MCM: Failed to find patch method {patch.Type.FullName}.{patch.MethodName}");
            }

            return methodInfo;
        }

        public class MethodDefinition
        {
            public MethodDefinition(Type type, string methodName, 
                BindingFlags bindingFlags = BindingFlags.Default,
                Type[] argumentTypes = null)
            {
                Type = type;
                MethodName = methodName;
                BindingFlags = bindingFlags;
                ArgumentTypes = argumentTypes;
            }

            public Type Type { get; }
            public string MethodName { get; }

            public BindingFlags BindingFlags { get; }
            
            public Type[] ArgumentTypes { get; }
        }
    }
}
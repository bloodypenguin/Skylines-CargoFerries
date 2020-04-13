using System;
using System.Collections.Generic;
using System.Reflection;

namespace CargoFerries.Utils
{
    public static class PrefabUtil
    {
        public static void TryCopyAttributes(PrefabAI src, PrefabAI dst, bool safe = true)
        {
            var oldAIFields = src.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var newAIFields = dst.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            var newAIFieldDic = new Dictionary<string, FieldInfo>(newAIFields.Length);
            foreach (var field in newAIFields)
            {
                newAIFieldDic.Add(field.Name, field);
            }

            foreach (var fieldInfo in oldAIFields)
            {
                // do not copy attributes marked NonSerialized
                bool copyField = !fieldInfo.IsDefined(typeof(NonSerializedAttribute), true);
                
                if (safe && !fieldInfo.IsDefined(typeof(CustomizablePropertyAttribute), true)) copyField = false;

                if (copyField)
                {
                    FieldInfo newAIField;
                    newAIFieldDic.TryGetValue(fieldInfo.Name, out newAIField);
                    try
                    {
                        if (newAIField != null && newAIField.GetType().Equals(fieldInfo.GetType()))
                        {
                            newAIField.SetValue(dst, fieldInfo.GetValue(src));
                        }
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
            }
        }
    }
}
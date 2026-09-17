using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameOn.GameSaver.Editor.SaveEditor
{
    internal sealed class SaveEditorAutoDetector
    {
        private readonly SaveEditorTypeInfo _typeInfo;
        private readonly Dictionary<Type, string[]> _serializedFieldNameCache = new Dictionary<Type, string[]>();

        public SaveEditorAutoDetector(SaveEditorTypeInfo typeInfo)
        {
            _typeInfo = typeInfo;
        }

        public bool TryDetect(string json, out SaveEditorDataScope scope)
        {
            scope = default;

            bool saveMatch = _typeInfo.HasSave && JsonMatchesType(json, _typeInfo.SaveType);
            bool profileMatch = _typeInfo.HasProfile && JsonMatchesType(json, _typeInfo.ProfileType);

            if (saveMatch == profileMatch)
            {
                return false;
            }

            scope = saveMatch ? SaveEditorDataScope.Save : SaveEditorDataScope.Profile;
            return true;
        }

        private bool JsonMatchesType(string json, Type type)
        {
            if (string.IsNullOrEmpty(json) || type == null)
            {
                return false;
            }

            if (DoesJsonContainKnownField(json, type))
            {
                return true;
            }

            if (!TryInstantiate(type, out object instance))
            {
                return false;
            }

            string before = JsonUtility.ToJson(instance);
            JsonUtility.FromJsonOverwrite(json, instance);
            string after = JsonUtility.ToJson(instance);

            return !string.Equals(before, after, StringComparison.Ordinal);
        }

        private bool DoesJsonContainKnownField(string json, Type type)
        {
            foreach (var fieldName in GetSerializedFieldNames(type))
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    continue;
                }

                if (json.IndexOf($"\"{fieldName}\"", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private string[] GetSerializedFieldNames(Type type)
        {
            if (type == null)
            {
                return Array.Empty<string>();
            }

            if (_serializedFieldNameCache.TryGetValue(type, out var cached))
            {
                return cached;
            }

            var names = new HashSet<string>(StringComparer.Ordinal);
            CollectSerializedFieldNames(type, names);

            var result = names.ToArray();
            _serializedFieldNameCache[type] = result;
            return result;
        }

        private static void CollectSerializedFieldNames(Type type, HashSet<string> aggregate)
        {
            if (type == null || type == typeof(object))
            {
                return;
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var field in type.GetFields(flags))
            {
                if (field.IsStatic)
                {
                    continue;
                }

                if (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField)))
                {
                    aggregate.Add(field.Name);
                }
            }

            CollectSerializedFieldNames(type.BaseType, aggregate);
        }

        private static bool TryInstantiate(Type type, out object instance)
        {
            try
            {
                instance = Activator.CreateInstance(type);
                return instance != null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to create instance of {type}: {ex.Message}");
                instance = null;
                return false;
            }
        }
    }
}

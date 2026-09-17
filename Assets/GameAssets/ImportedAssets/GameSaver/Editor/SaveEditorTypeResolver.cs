using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameSaver;

namespace GameSaver.Editor.SaveEditor
{
    internal readonly struct SaveEditorTypeInfo
    {
        public Type SaveType { get; }
        public Type ProfileType { get; }

        public SaveEditorTypeInfo(Type saveType, Type profileType)
        {
            SaveType = saveType;
            ProfileType = profileType;
        }

        public bool HasSave => SaveType != null;
        public bool HasProfile => ProfileType != null;
        public bool HasAny => HasSave || HasProfile;
    }

    internal static class SaveEditorTypeResolver
    {
        public static SaveEditorTypeInfo Resolve()
        {
            var candidates = EnumerateImplementations().ToList();
            if (candidates.Count == 0)
            {
                return new SaveEditorTypeInfo(null, null);
            }

            var preferred = candidates.FirstOrDefault(c => string.Equals(c.Saver.Assembly.GetName().Name, "Assembly-CSharp", StringComparison.Ordinal));
            if (preferred == default)
            {
                preferred = candidates.FirstOrDefault(c => string.Equals(c.Saver.Name, "GameSaver", StringComparison.Ordinal));
            }

            if (preferred == default)
            {
                preferred = candidates[0];
            }

            return new SaveEditorTypeInfo(preferred.Save, preferred.Profile);
        }

        private static IEnumerable<(Type Saver, Type Save, Type Profile)> EnumerateImplementations()
        {
            Type genericDefinition = typeof(GameSaverGeneric<,>);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in GetTypesSafe(assembly))
                {
                    if (type == null || type.IsAbstract || type.IsGenericTypeDefinition)
                    {
                        continue;
                    }

                    if (TryGetGenericArguments(type, genericDefinition, out var arguments))
                    {
                        yield return (type, arguments[0], arguments[1]);
                    }
                }
            }
        }

        private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
        }

        private static bool TryGetGenericArguments(Type candidate, Type definition, out Type[] arguments)
        {
            if (candidate == null)
            {
                arguments = null;
                return false;
            }

            if (candidate.IsGenericType && candidate.GetGenericTypeDefinition() == definition)
            {
                arguments = candidate.GetGenericArguments();
                return true;
            }

            return TryGetGenericArguments(candidate.BaseType, definition, out arguments);
        }
    }
}

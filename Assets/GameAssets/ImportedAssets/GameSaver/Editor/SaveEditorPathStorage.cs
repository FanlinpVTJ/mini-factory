using System;
using UnityEngine;

namespace GameSaver.Editor.SaveEditor
{
    internal sealed class SaveEditorPathStorage
    {
        private const string PrefsKeyPrefix = "UtilitiesSaveEditorPath";

        private readonly SaveEditorTypeInfo _typeInfo;

        public SaveEditorPathStorage(SaveEditorTypeInfo typeInfo)
        {
            _typeInfo = typeInfo;
        }

        public string Load(SaveEditorDataScope scope)
        {
            string key = BuildKey(scope);
            return PlayerPrefs.GetString(key, string.Empty);
        }

        public void Store(SaveEditorDataScope scope, string path)
        {
            string key = BuildKey(scope);
            if (string.IsNullOrEmpty(path))
            {
                PlayerPrefs.DeleteKey(key);
            }
            else
            {
                PlayerPrefs.SetString(key, path);
            }

            PlayerPrefs.Save();
        }

        private string BuildKey(SaveEditorDataScope scope)
        {
            Type type = scope == SaveEditorDataScope.Save ? _typeInfo.SaveType : _typeInfo.ProfileType;
            string typeId = type != null ? type.AssemblyQualifiedName : scope.ToString();
            return $"{PrefsKeyPrefix}:{typeId}:{scope}";
        }
    }
}

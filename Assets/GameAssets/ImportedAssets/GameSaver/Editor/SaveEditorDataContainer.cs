using System;
using UnityEngine;

namespace GameSaver.Editor.SaveEditor
{
    [Serializable]
    internal class SaveEditorDataContainer : ScriptableObject
    {
        internal const string SavePropertyName = "_save";
        internal const string ProfilePropertyName = "_profile";

        [SerializeReference] private object _save;
        [SerializeReference] private object _profile;
        [SerializeField] private bool _hasSaveData;
        [SerializeField] private bool _hasProfileData;

        [NonSerialized] private Type _saveType;
        [NonSerialized] private Type _profileType;

        public void Initialize(Type saveType, Type profileType)
        {
            _saveType = saveType;
            _profileType = profileType;
            EnsureInstances();
        }

        public bool HasType(SaveEditorDataScope scope)
        {
            return scope == SaveEditorDataScope.Save ? _saveType != null : _profileType != null;
        }

        public bool HasData(SaveEditorDataScope scope)
        {
            return scope == SaveEditorDataScope.Save ? _hasSaveData : _hasProfileData;
        }

        public void EnsureDataInitialized(SaveEditorDataScope scope)
        {
            EnsureInstances();

            if (scope == SaveEditorDataScope.Save && _save == null && _saveType != null)
            {
                _save = Activator.CreateInstance(_saveType);
            }

            if (scope == SaveEditorDataScope.Profile && _profile == null && _profileType != null)
            {
                _profile = Activator.CreateInstance(_profileType);
            }
        }

        public void LoadFromJson(string json, SaveEditorDataScope scope)
        {
            EnsureDataInitialized(scope);
            object target = GetData(scope);
            if (target == null)
            {
                Debug.LogError($"Unable to instantiate data for {scope}.");
                return;
            }

            if (!string.IsNullOrEmpty(json))
            {
                JsonUtility.FromJsonOverwrite(json, target);
            }

            SetHasData(scope, true);
        }

        public string ToJson(bool prettyPrint, SaveEditorDataScope scope)
        {
            object target = GetData(scope);
            if (target == null || !HasData(scope))
            {
                return string.Empty;
            }

            return JsonUtility.ToJson(target, prettyPrint);
        }

        public object GetData(SaveEditorDataScope scope)
        {
            return scope == SaveEditorDataScope.Save ? _save : _profile;
        }

        private void EnsureInstances()
        {
            if (_saveType != null && (_save == null || _save.GetType() != _saveType))
            {
                _save = Activator.CreateInstance(_saveType);
                _hasSaveData = false;
            }

            if (_profileType != null && (_profile == null || _profile.GetType() != _profileType))
            {
                _profile = Activator.CreateInstance(_profileType);
                _hasProfileData = false;
            }
        }

        private void SetHasData(SaveEditorDataScope scope, bool value)
        {
            if (scope == SaveEditorDataScope.Save)
            {
                _hasSaveData = value;
            }
            else
            {
                _hasProfileData = value;
            }
        }
    }
}

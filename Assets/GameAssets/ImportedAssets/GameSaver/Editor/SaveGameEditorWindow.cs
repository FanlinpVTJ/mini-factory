using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameOn.GameSaver.Editor.SaveEditor
{
    public class SaveGameEditorWindow : EditorWindow
    {
        private const string WindowTitle = "Save Game Editor";

        [SerializeField] private SaveEditorDataContainer _container;

        private SerializedObject _serializedContainer;
        private SerializedProperty _saveProperty;
        private SerializedProperty _profileProperty;

        private SaveEditorTypeInfo _typeInfo;
        private SaveEditorPathStorage _pathStorage;
        private SaveEditorAutoDetector _autoDetector;

        private bool _typesResolved;
        private bool _pathsInitialized;
        private bool _autoSaveLoaded;
        private bool _autoProfileLoaded;

        private string _currentSavePath = string.Empty;
        private string _currentProfilePath = string.Empty;

        private SaveEditorDataScope _activeScope = SaveEditorDataScope.Save;
        private Vector2 _scroll;

        [MenuItem("Tools/GameSaver/Save Game Editor")]
        private static void OpenWindow()
        {
            var window = GetWindow<SaveGameEditorWindow>();
            window.titleContent = new GUIContent(WindowTitle);
            window.Show();
        }

        private void OnEnable()
        {
            EnsureContext();
        }

        private void OnDisable()
        {
            if (_container != null)
            {
                DestroyImmediate(_container);
                _container = null;
            }

            _serializedContainer = null;
            _saveProperty = null;
            _profileProperty = null;
        }

        private void OnGUI()
        {
            if (!EnsureContext())
            {
                EditorGUILayout.HelpBox("Could not locate a GameSaverGeneric<,> implementation. Add a class that derives from GameSaverGeneric<,> to use this editor.", MessageType.Warning);
                return;
            }

            DrawFileControls();
            EditorGUILayout.Space();

            if (!_container.HasType(_activeScope))
            {
                EditorGUILayout.HelpBox("No data type has been registered for the selected scope.", MessageType.Info);
                return;
            }

            if (!_container.HasData(_activeScope))
            {
                EditorGUILayout.HelpBox($"Load a {_activeScope} json file to inspect and edit its contents.", MessageType.Info);
                return;
            }

            DrawDataInspector();
        }

        private bool EnsureContext()
        {
            if (!_typesResolved)
            {
                _typeInfo = SaveEditorTypeResolver.Resolve();
                _typesResolved = true;

                if (!_typeInfo.HasAny)
                {
                    return false;
                }

                _pathStorage = new SaveEditorPathStorage(_typeInfo);
                _autoDetector = new SaveEditorAutoDetector(_typeInfo);
            }

            EnsureContainer();
            return true;
        }

        private void EnsureContainer()
        {
            if (!_typeInfo.HasAny)
            {
                return;
            }

            if (_container == null)
            {
                _container = CreateInstance<SaveEditorDataContainer>();
                _container.hideFlags = HideFlags.DontSave;
            }

            _container.Initialize(_typeInfo.SaveType, _typeInfo.ProfileType);
            EnsureSerializedContainer();
            EnsurePathsInitialized();
            EnsureAutoLoad();
        }

        private void EnsureSerializedContainer()
        {
            if (_container == null)
            {
                return;
            }

            if (_serializedContainer == null || _serializedContainer.targetObject != _container)
            {
                _serializedContainer = new SerializedObject(_container);
                _saveProperty = _serializedContainer.FindProperty(SaveEditorDataContainer.SavePropertyName);
                _profileProperty = _serializedContainer.FindProperty(SaveEditorDataContainer.ProfilePropertyName);
            }
        }

        private void EnsurePathsInitialized()
        {
            if (_pathsInitialized || _pathStorage == null)
            {
                return;
            }

            if (_typeInfo.HasSave)
            {
                _currentSavePath = _pathStorage.Load(SaveEditorDataScope.Save);
            }

            if (_typeInfo.HasProfile)
            {
                _currentProfilePath = _pathStorage.Load(SaveEditorDataScope.Profile);
            }

            _pathsInitialized = true;
        }

        private void EnsureAutoLoad()
        {
            if (_typeInfo.HasSave && !_autoSaveLoaded && !string.IsNullOrEmpty(_currentSavePath))
            {
                _autoSaveLoaded = true;
                TryAutoLoad(SaveEditorDataScope.Save, _currentSavePath);
            }

            if (_typeInfo.HasProfile && !_autoProfileLoaded && !string.IsNullOrEmpty(_currentProfilePath))
            {
                _autoProfileLoaded = true;
                TryAutoLoad(SaveEditorDataScope.Profile, _currentProfilePath);
            }
        }

        private void TryAutoLoad(SaveEditorDataScope scope, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (File.Exists(path))
            {
                LoadData(scope, path, showDialogs: false, allowAutoDetect: false);
            }
            else
            {
                SetCurrentPath(scope, string.Empty);
            }
        }

        private void DrawFileControls()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Data Set");
                    SaveEditorDataScope newScope = _typeInfo.HasProfile
                        ? (SaveEditorDataScope)EditorGUILayout.EnumPopup(_activeScope)
                        : SaveEditorDataScope.Save;

                    if (newScope != _activeScope)
                    {
                        _activeScope = newScope;
                    }

                    EditorGUILayout.LabelField(GetDataType(_activeScope)?.Name ?? "Unknown", EditorStyles.miniLabel, GUILayout.MaxWidth(200));
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("File Path");
                    string currentPath = GetCurrentPath(_activeScope);

                    EditorGUI.BeginChangeCheck();
                    string newPath = EditorGUILayout.TextField(currentPath);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetCurrentPath(_activeScope, newPath);
                    }

                    if (GUILayout.Button("...", GUILayout.Width(30)))
                    {
                        string directory = string.IsNullOrEmpty(currentPath) ? Application.persistentDataPath : Path.GetDirectoryName(currentPath);
                        string path = EditorUtility.OpenFilePanel("Select json file", directory, "json");
                        if (!string.IsNullOrEmpty(path))
                        {
                            LoadData(_activeScope, path, showDialogs: true, allowAutoDetect: true);
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    bool hasExistingFile = File.Exists(GetCurrentPath(_activeScope));

                    EditorGUI.BeginDisabledGroup(!hasExistingFile);
                    if (GUILayout.Button("Load"))
                    {
                        LoadData(_activeScope, GetCurrentPath(_activeScope), showDialogs: true, allowAutoDetect: true);
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(!_container.HasData(_activeScope));
                    if (GUILayout.Button("Save"))
                    {
                        SaveToFile(GetCurrentPath(_activeScope));
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(!_container.HasData(_activeScope));
                    if (GUILayout.Button("Save As..."))
                    {
                        SaveAsNewFile();
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        private void DrawDataInspector()
        {
            EnsureSerializedContainer();

            _container.EnsureDataInitialized(_activeScope);
            _serializedContainer.Update();

            SerializedProperty property = GetActiveProperty();
            if (property == null)
            {
                EditorGUILayout.HelpBox("Unable to access the serialized property for the current data scope.", MessageType.Warning);
                return;
            }

            using (var scrollScope = new EditorGUILayout.ScrollViewScope(_scroll))
            {
                EditorGUILayout.PropertyField(property, includeChildren: true);
                _scroll = scrollScope.scrollPosition;
            }

            _serializedContainer.ApplyModifiedProperties();
        }

        private void LoadData(SaveEditorDataScope scope, string path, bool showDialogs, bool allowAutoDetect)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                if (showDialogs)
                {
                    EditorUtility.DisplayDialog(WindowTitle, "Selected file cannot be found.", "OK");
                }

                SetCurrentPath(scope, string.Empty);
                return;
            }

            string json;
            try
            {
                json = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to read {scope} file: {ex}");
                if (showDialogs)
                {
                    EditorUtility.DisplayDialog(WindowTitle, "Could not read the selected file. See console for details.", "OK");
                }

                return;
            }

            SaveEditorDataScope scopeToLoad = scope;
            if (allowAutoDetect && _autoDetector != null && _typeInfo.HasSave && _typeInfo.HasProfile && _autoDetector.TryDetect(json, out var detectedScope))
            {
                scopeToLoad = detectedScope;

                if (_activeScope != scopeToLoad)
                {
                    _activeScope = scopeToLoad;
                    ShowNotification(new GUIContent($"Loaded as {scopeToLoad}"));
                }
            }

            try
            {
                _container.LoadFromJson(json, scopeToLoad);
                SetCurrentPath(scopeToLoad, path);

                if (scopeToLoad == SaveEditorDataScope.Save)
                {
                    _autoSaveLoaded = true;
                }
                else
                {
                    _autoProfileLoaded = true;
                }

                EnsureSerializedContainer();
                _serializedContainer?.Update();
                Repaint();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to deserialize {scopeToLoad} file: {ex}");
                if (showDialogs)
                {
                    EditorUtility.DisplayDialog(WindowTitle, "Could not deserialize the selected file. See console for details.", "OK");
                }
            }
        }

        private void SaveToFile(string path)
        {
            if (!_container.HasData(_activeScope))
            {
                EditorUtility.DisplayDialog(WindowTitle, $"Nothing to save for {_activeScope}.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                SaveAsNewFile();
                return;
            }

            EnsureSerializedContainer();
            _serializedContainer.ApplyModifiedProperties();

            try
            {
                string json = _container.ToJson(true, _activeScope);
                File.WriteAllText(path, json);
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write {_activeScope} file: {ex}");
                EditorUtility.DisplayDialog(WindowTitle, "Could not write the file. See console for details.", "OK");
            }
        }

        private void SaveAsNewFile()
        {
            if (!_container.HasData(_activeScope))
            {
                EditorUtility.DisplayDialog(WindowTitle, $"Nothing to save for {_activeScope}.", "OK");
                return;
            }

            string currentPath = GetCurrentPath(_activeScope);
            string directory = string.IsNullOrEmpty(currentPath) ? Application.persistentDataPath : Path.GetDirectoryName(currentPath);
            string defaultName = $"{GetDataType(_activeScope)?.Name ?? _activeScope.ToString()}.json";

            string path = EditorUtility.SaveFilePanel("Save edited file", directory, defaultName, "json");
            if (!string.IsNullOrEmpty(path))
            {
                SetCurrentPath(_activeScope, path);
                SaveToFile(path);
            }
        }

        private SerializedProperty GetActiveProperty()
        {
            return _activeScope == SaveEditorDataScope.Save ? _saveProperty : _profileProperty;
        }

        private Type GetDataType(SaveEditorDataScope scope)
        {
            return scope == SaveEditorDataScope.Save ? _typeInfo.SaveType : _typeInfo.ProfileType;
        }

        private string GetCurrentPath(SaveEditorDataScope scope)
        {
            return scope == SaveEditorDataScope.Save ? _currentSavePath : _currentProfilePath;
        }

        private void SetCurrentPath(SaveEditorDataScope scope, string path, bool persist = true)
        {
            string normalized = path ?? string.Empty;

            if (string.Equals(GetCurrentPath(scope), normalized, StringComparison.Ordinal))
            {
                return;
            }

            if (scope == SaveEditorDataScope.Save)
            {
                _currentSavePath = normalized;
            }
            else
            {
                _currentProfilePath = normalized;
            }

            if (persist)
            {
                _pathStorage?.Store(scope, normalized);
            }
        }
    }
}

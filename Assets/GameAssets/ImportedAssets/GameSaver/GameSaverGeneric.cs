using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
namespace GameSaver
{

    public class GameSaverGeneric<TSave, TProfile> : GameSaverBase where TSave : SaveGameBase, new() where TProfile : new()
    {
		#region Fields

		private const string DISK_FULL_ANDROID = "Disk full";
		private const string DISK_FULL_IOS = "No space left on device";
        private const string GAMESAVER_NAME = "[GameSaver]";

		public static event Action OnDiscSpaceFullSaveFailed;
		public static event Action OnBeforeSavingSave;
        public static event Action OnSavedSave;
        public static event Action OnProfilesUpdated;

        protected static TSave _save;
        protected static TProfile _activeProfile;
        protected static GameSaverMonoListener _gameSaverMonoListener;
        protected static bool _saveQueued;
        private static bool _isQuitting;

        #endregion

        #region Properties

        public static IReadOnlyList<string> ProfileNames => Save.Profiles;
        public static TSave Save
        {
            get
            {
                if (_save == null)
                {
                    LoadSaveFromFile();
                }

                return _save;
            }
        }


        public static TProfile Profile
        {
            get
            {
                if (_activeProfile == null)
                {
                    SetSelection(Save.ActiveProfileName, LoadProfileFromFile(Save.ActiveProfileName));
                }
                return _activeProfile;
            }
        }

        public static string ActiveProfileName => Save.ActiveProfileName;

        #endregion

        public static void SaveChanged()
        {
            _saveQueued = true;
            if (Application.isPlaying)
            {
                if (_isQuitting)
                {
                    ForceSave();
                    return;
                }
                if (_gameSaverMonoListener == null)
                {
                    _gameSaverMonoListener = new GameObject("GameSaverMonoListener").AddComponent<GameSaverMonoListener>();
                    UnityEngine.Object.DontDestroyOnLoad(_gameSaverMonoListener.gameObject);
                    _gameSaverMonoListener.OnUpdate += CheckSave;
                    Application.quitting += OnQuit;
                }
            }
            else
            {
                ForceSave();
            }

        }

        private static void CheckSave()
        {
            if (_saveQueued)
            {
                ForceSave();
            }
        }

        private static void OnQuit()
        {
            Application.quitting -= OnQuit;
            _isQuitting = true;
        }

		private static bool IsDiskFullIOException(IOException ex)
		{
			var msg = ex.Message ?? string.Empty;
			return msg.Contains(DISK_FULL_ANDROID, StringComparison.OrdinalIgnoreCase) ||
				   msg.Contains(DISK_FULL_IOS, StringComparison.OrdinalIgnoreCase);
		}

		public static void ForceSave()
		{
			try 
            { 
                OnBeforeSavingSave?.Invoke(); 
            }
			catch (Exception e) 
            { 
                Debug.LogException(e); 
            }

			try
			{
                if (_activeProfile != null)
                {
                    SaveCurrentProfileToDisc(Save.ActiveProfileName);
                }
				SaveToDisc();
				_saveQueued = false;
			}
			catch (IOException ioEx) when (IsDiskFullIOException(ioEx))
			{
#if UNITY_ANDROID
                Debug.LogError(GAMESAVER_NAME + DISK_FULL_ANDROID);
#elif UNITY_IOS
                Debug.LogError(GAMESAVER_NAME + DISK_FULL_IOS);
#else
                Debug.LogError(GAMESAVER_NAME + DISK_FULL_ANDROID);
#endif
                OnDiscSpaceFullSaveFailed?.Invoke();
			}

			catch (Exception e) 
            { 
                Debug.LogException(e); 
            }

			try 
            { 
                OnSavedSave?.Invoke(); 
            }
			catch (Exception e) 
            { 
                Debug.LogException(e); 
            }
		}

        private static void SetSelection(string name, TProfile profile)
        {
            Save.ActiveProfileName = name;
            _activeProfile = profile;
            SaveChanged();
            OnProfilesUpdated?.Invoke();
        }

        public static bool IsProfileExist(string name)
        {
            string fileName = GetProfileFileName(name);
            string path = Path.Combine(Settings.SavesFullDirectory, fileName) + ".json";

            if (Directory.Exists(Settings.SavesFullDirectory) && File.Exists(path))
                return true;
            return false;
        }

        public static bool CurrentProfileExists()
        {
            return IsProfileExist(Save.ActiveProfileName);
        }

        public static TProfile GetProfile(string name)
        {
            if (name == Save.ActiveProfileName)
            {
                return _activeProfile;
            }
            return LoadProfileFromFile(name);
        }

        public static void CreateNewProfile(string name, bool autoSelect = true)
        {
            if (Save.Profiles.Contains(name))
            {
                Debug.Log("Profile already exists: " + name);
                return;
            }
            var profile = LoadProfileFromFile(name);
            if (autoSelect)
            {
                SetSelection(name, profile);
            }
            else
            {
                OnProfilesUpdated?.Invoke();
            }
        }

        public static void SelectProfile(string name)
        {
            if (!Save.Profiles.Contains(name))
            {
                Debug.LogError("Profile doesn't exist: " + name);
                return;
            }
            SetSelection(name, LoadProfileFromFile(name));
        }

        public static bool DeleteActiveProfile()
        {
            return TryDeleteProfile(Save.ActiveProfileName);
        }

        public static bool TryRenameCurrentProfile(string name)
        {
            if (Save.Profiles.Contains(name))
            {
                return false;
            }
            string oldName = Save.ActiveProfileName;

            SaveCurrentProfileToDisc(name);
            SetSelection(name, _activeProfile);
            if (IsProfileExist(oldName) && oldName != name)
                TryDeleteProfile(oldName);
            return true;
        }

        public static bool CanDeleteProfile(string name)
        {
            if (Save.Profiles.Count == 1)
                return false;
            string fileName = GetProfileFileName(name);
            string path = Path.Combine(Settings.SavesFullDirectory, fileName) + ".json";
            if (!Directory.Exists(Settings.SavesFullDirectory) || !File.Exists(path))
                return false;
            return true;
        }

        public static bool TryDeleteProfile(string name)
        {
            if (Save.Profiles.Count == 1)
            {
                Debug.LogError("Can't delete last profile");
                return false;
            }

            string fileName = GetProfileFileName(name);
            string path = Path.Combine(Settings.SavesFullDirectory, fileName) + ".json";

            if (!Directory.Exists(Settings.SavesFullDirectory) || !File.Exists(path))
                return false;

            int index = Save.Profiles.IndexOf(name);
            File.Delete(path);
            Save.Profiles.Remove(name);

            if (Save.ActiveProfileName == name)
            {
                SelectProfile(Save.Profiles[index < Save.Profiles.Count ? index : Save.Profiles.Count - 1]);
            }
            else
            {
                SaveChanged();
                OnProfilesUpdated?.Invoke();
            }
            return true;
        }


        private static int GetEncryptionKeyWithCurrentSettings()
        {
            if (Debug.isDebugBuild)
            {
                if (!Settings.EncryptInDevBuilds)
                {
                    return 0;
                }
            }
            else
            {
                if (!Settings.EncryptInRelease)
                {
                    return 0;
                }
            }

            return Settings.EncryptionKey;
        }

        private static void LoadSaveFromFile()
        {
            LoadSaveFromDisc();
            if (_save == null)
            {
                _save = new TSave();
                SaveToDisc();
            }
        }

        private static TProfile LoadProfileFromFile(string name)
        {
            TProfile profile;

            profile = LoadProfileFromDisc(name);

            if (profile == null)
            {
                profile = new TProfile();
                SaveProfileToDisc(profile, name);
            }
            if (!Save.Profiles.Contains(name))
            {
                Save.Profiles.Add(name);
                SaveChanged();
            }

            return profile;
        }

        private static void SaveToDisc()
        {
            if (_save is ISaveLoadReciever saveLoadReciever)
            {
                saveLoadReciever.OnBeforeSerialize();
            }

            if (Application.isEditor && Settings.SaveOverride != null)
            {
                (Settings.SaveOverride as ScriptableSaveDataGeneric<TSave>).SaveObject = _save;
                Settings.SaveOverride.SetDirty();
            }
            else
            {
                JsonSaver.EncryptionKey = GetEncryptionKeyWithCurrentSettings();
                JsonSaver.SaveToJson(_save, Settings.SavesFullDirectory, Settings.SaveName);
            }
        }

        private static void LoadSaveFromDisc()
        {
            if (Application.isEditor && Settings.SaveOverride != null)
            {
                _save = (Settings.SaveOverride as ScriptableSaveDataGeneric<TSave>).SaveObject;
            }
            else
            {
                JsonSaver.EncryptionKey = GetEncryptionKeyWithCurrentSettings();
                _save = JsonSaver.LoadJson<TSave>(Settings.SavesFullDirectory, Settings.SaveName);
            }

            if (_save is ISaveLoadReciever saveLoadReciever)
            {
                saveLoadReciever.OnAfterDeserialize();
            }
        }

        private static void SaveCurrentProfileToDisc(string name)
        {
            SaveProfileToDisc(_activeProfile, name);
        }

        private static void SaveProfileToDisc(TProfile profileObj, string name)
        {
            if (profileObj is ISaveLoadReciever saveLoadReciever)
            {
                saveLoadReciever.OnBeforeSerialize();
            }

            if (!Save.Profiles.Contains(name))
            {
                Save.Profiles.Add(name);
                SaveChanged();
            }

            if (Application.isEditor && Settings.TryGetProfileOverride(name, out ScriptableSaveDataGeneric<TProfile> profile))
            {
                profile.SaveObject = profileObj;
                profile.SetDirty();
            }
            else
            {
                JsonSaver.EncryptionKey = GetEncryptionKeyWithCurrentSettings();
                JsonSaver.SaveToJson(profileObj, Settings.SavesFullDirectory, GetProfileFileName(name));
            }

        }

        private static TProfile LoadProfileFromDisc(string name)
        {
            TProfile profileObj;

            if (Application.isEditor && Settings.TryGetProfileOverride(name, out ScriptableSaveDataGeneric<TProfile> profile))
            {
                profileObj = profile.SaveObject;
            }
            else
            {
                JsonSaver.EncryptionKey = GetEncryptionKeyWithCurrentSettings();
                profileObj = JsonSaver.LoadJson<TProfile>(Settings.SavesFullDirectory, GetProfileFileName(name));
            }


            if (profileObj is ISaveLoadReciever saveLoadReciever)
            {
                saveLoadReciever.OnAfterDeserialize();
            }

            return profileObj;
        }

        #region Texture Save-Load

        public static void SaveTexture(Texture2D texture, string name)
        {
            string directory = Settings.SavesFullDirectory;
            JsonSaver.CheckOrCreateDirectory(directory);

            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(directory, name) + ".png", bytes);
        }

        public static Texture2D LoadTexture(string name)
        {
            string path = Path.Combine(Settings.SavesFullDirectory, name) + ".png";

            if (File.Exists(path))
            {
                var bytes = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(bytes);
                return tex;
            }

            return null;
        }

        #endregion
    }
}

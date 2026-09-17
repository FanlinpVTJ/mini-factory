using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace GameSaver
{
    [CreateAssetMenu(fileName = "GameSaverSettings", menuName = "Game Saver/Game Saver Settings", order = 131)]
    public class GameSaverSettings : ScriptableObject
    {
        [System.Serializable]
        public class ProfileOverride
        {
            public string Name;
            public ScriptableSaveDataBase Profile;
        }

        #region Fields

        [SerializeField]
        [Tooltip("Save file name without \".json\"")]
        private string _saveName = "SData";

        [SerializeField]
        [Tooltip("Profile file prefix")]
        private string _profilePrefix = "Profile_";

        [SerializeField]
        [Tooltip("What profile name to use if no profile name was set from code")]
        private string _defaultProfileName = "Default";

        [SerializeField]
        [Tooltip("Directory relative to Application.presistentDataPath")]
        private string _savesRelativeDirectory = "Prog";

        [SerializeField]
        [Tooltip("Save load to this save instead of one from disc. Editor Only")]
        private ScriptableSaveDataBase _saveOverride;

        [SerializeField]
        [Tooltip("Save load this profiles by names instead of disc. Editor Only")]
        private ProfileOverride[] _profileOverrides;

        [Header("Encryption")]
        [SerializeField]
        [Tooltip("Should json save file be encrypted in developement builds or in editor")]
        private bool _encryptInDevBuilds = false;

        [SerializeField]
        [Tooltip("Should json save file be encrypted in release builds")]
        private bool _encryptInRelease = true;

        [SerializeField]
        [Header("Use 0 to disable encryption completely")]
        [Tooltip("Use 0 to disable encryption completely")]
        private int _encryptionKey = -963;

        #endregion

        #region Properties

        private static GameSaverSettings _defaultSettings;
        ///<summary> Default game saving settings if no settings are presented in resources </summary>
        public static GameSaverSettings DefaultSettings
        {
            get
            {
                if (_defaultSettings == null)
                {
                    _defaultSettings = new GameSaverSettings();
                }

                return _defaultSettings;
            }
        }

        ///<summary> Save file name without ".json" </summary>
        public string SaveName { get => _saveName; set => _saveName = value; }
        /// <summary> Profile file prefix </summary>
        public string ProfilePrefix { get => _profilePrefix; }
        /// <summary> What profile name to use if no profile name was set from code </summary>
        public string DefaultProfileName { get => _defaultProfileName; }
        ///<summary> Directory relative to Application.presistentDataPath </summary>
        public string SavesRelativeDirectory { get => _savesRelativeDirectory; set => _savesRelativeDirectory = value; }
        ///<summary> Directory where saves are located. Without file name </summary>
        public string SavesFullDirectory => Path.Combine(Application.persistentDataPath, _savesRelativeDirectory);
        ///<summary> Path of save file with file name and ".json" </summary>
        public string SavePath => Path.Combine(SavesFullDirectory, SaveName) + ".json";

        /// <summary> Should json save file be encrypted in developement builds or in editor </summary>
        public bool EncryptInDevBuilds { get => _encryptInDevBuilds; }
        /// <summary> Should json save file be encrypted in release builds </summary>
        public bool EncryptInRelease { get => _encryptInRelease; }
        /// <summary> 0 will disable encryption completely </summary>
        public int EncryptionKey { get => _encryptionKey; }
        
        public ScriptableSaveDataBase SaveOverride { get => _saveOverride; }
        public ProfileOverride[] ProfileOverrides { get => _profileOverrides; }
        
        #endregion

        internal bool TryGetProfileOverride<T>(string name, out ScriptableSaveDataGeneric<T> profile) where T : new()
        {
            for (int i = 0; i < _profileOverrides.Length; i++)
            {
                if (_profileOverrides[i].Name == name)
                {
                    if (_profileOverrides[i].Profile is ScriptableSaveDataGeneric<T> scriptableSaveData)
                    {
                        profile = scriptableSaveData;
                        return true;
                    }
                }
            }

            profile = default;
            return false;
        }
    }
}

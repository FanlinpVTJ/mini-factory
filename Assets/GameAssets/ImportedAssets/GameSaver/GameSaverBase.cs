using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GameSaver
{
    public class GameSaverBase
    {
        protected static event System.Action OnSavesDeleted;

        #region Fields

        private static GameSaverSettings _settings;
        protected static string _currentProfileName;

        #endregion

        #region Properties

        public static GameSaverSettings Settings { get => GetSettings(); }

        #endregion

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/GameSaver/Delete All Saves")]
#endif
        public static void DeleteAllSaves()
        {
            DeleteSaves();
            OnSavesDeleted?.Invoke();
        }

#if UNITY_EDITOR_WIN

        [UnityEditor.MenuItem("Tools/GameSaver/Open Saves Directory")]
        public static void OpenSavesDirectory()
        {
            var path = Settings.SavesFullDirectory;
            JsonSaver.CheckOrCreateDirectory(path);

            path = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
            System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
        }

#endif

        public static void DeleteSaves()
        {
            var directory = Settings.SavesFullDirectory;

            if (Directory.Exists(directory))
            {
                var dirInfo = new DirectoryInfo(directory);

                foreach (FileInfo fileInfo in dirInfo.GetFiles())
                {
                    fileInfo.Delete();
                }
            }
        }

        protected static string GetProfileFileName(string profileName)
        {
            return $"{Settings.ProfilePrefix}{profileName}";
        }

        private static GameSaverSettings GetSettings()
        {
            if (_settings == null)
            {
                var settings = Resources.Load<GameSaverSettings>("GameSaverSettings");
                _settings = settings == null ? GameSaverSettings.DefaultSettings : settings;
            }

            return _settings;
        }
    }
}

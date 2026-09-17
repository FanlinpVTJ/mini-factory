using System;
using System.IO;
using GameSaver;

namespace GameSaver
{
    public class GameSaver : GameSaverGeneric<SaveGame, PlayerProfile>
    {
        public static PlayerProfile GetActiveProfile() => Profile;

        public static void SaveActiveProfile() => SaveChanged();
    }
}

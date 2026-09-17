using System.Collections.Generic;
using UnityEngine;

namespace GameOn.GameSaver
{
    [System.Serializable]
    public class SaveGameBase
    {
        private const string DefaultProfileName = "Player";

        [SerializeField]
        internal string ActiveProfileName = DefaultProfileName;
        [SerializeField]
        internal List<string> Profiles = new List<string>();
    }
}

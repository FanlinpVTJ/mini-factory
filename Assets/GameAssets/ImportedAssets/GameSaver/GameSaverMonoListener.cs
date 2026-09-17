using UnityEngine;
using System;


namespace GameOn.GameSaver
{
    public class GameSaverMonoListener: MonoBehaviour
    {
        public event Action OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();   
        }
    }
}

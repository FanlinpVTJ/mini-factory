using UnityEngine;

namespace TweenComponents
{
    [System.Serializable]
    public class TweenExecutionSettings
    {
        [Tooltip("If == false, tween can`t be executed if it`s already being played.")]
        public bool CanBeInterrupted = true;


        [Space]
        public bool ExecuteOnEnable;
        [Tooltip("If PlayOnEnable, throws this value in Execute method.")]
        public bool StraightOnEnable = true;

        [Space]
        public bool ResetValueOnExecute = true;

        [Space]
        public bool IgnoreTimeScale;
    }
}
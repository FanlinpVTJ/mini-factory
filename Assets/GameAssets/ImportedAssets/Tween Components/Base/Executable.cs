using System;
using UnityEngine;

namespace TweenComponents.Base
{
    public class Executable : MonoBehaviour
    {
        public bool IsPlaying { get; protected set; }
        public bool IsStraight { get; protected set; }
        public bool IsCompleted { get; protected set; }

        public event Action OnStarted;
        public event Action OnCompleted;

        public virtual void Execute(bool straight = true)
        {
            IsStraight = straight;
            IsPlaying = true;
            IsCompleted = false;

            OnStarted?.Invoke();
        }
        public virtual void Stop()
        {
            IsPlaying = false;
            IsCompleted = false;
        }
        public virtual void Kill()
        {
            IsPlaying = false;
            IsCompleted = false;
        }

        protected internal virtual void OnExecutionCompleted()
        {
            IsPlaying = false;
            IsCompleted = true;

            OnCompleted?.Invoke();
        }

#if UNITY_EDITOR

        [ContextMenu("Execute")]
        private void MenuExecuteStraight()
        {
            Execute();
        }

        [ContextMenu("Execute Reversed")]
        private void MenuExecuteReversed()
        {
            Execute(false);
        }

#endif
    }
}

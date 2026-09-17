using System.Collections.Generic;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class TweenSequence : Executable
    {
        [Header("Execution Settings")]
        public bool ExecuteOnEnable;
        public bool StraightOnEnable = true;
        public bool ThrowStraightStateInTweens = false;
        [Space]
        public bool CanBeInterrupted = true;
        [Space]
        public List<Executable> SequenceList;

        private int _currentIndex;

        public Executable CurrentExecutable { get; private set; }

        private void OnEnable()
        {
            if (ExecuteOnEnable)
            {
                Execute(StraightOnEnable);
            }
        }

        public override void Execute(bool straight = true)
        {
            if (IsPlaying && !CanBeInterrupted) return;
            if (SequenceList.Count < 1) return;

            Stop();

            base.Execute(straight);

            _currentIndex = IsStraight ? 0 : SequenceList.Count - 1;

            IsPlaying = true;
            IsCompleted = false;

            StartTween();
        }

        protected internal override void OnExecutionCompleted()
        {
            CurrentExecutable.OnCompleted -= OnExecutionCompleted;

            _currentIndex += IsStraight ? 1 : -1;

            if(_currentIndex < 0 || _currentIndex >= SequenceList.Count)
            {
                Stop();

                base.OnExecutionCompleted();
                return;
            }

            StartTween();
        }

        private void StartTween()
        {
            CurrentExecutable = SequenceList[_currentIndex];

            if(CurrentExecutable == this)
            {
                Debug.LogError($"Infinite loop! Sequence on object ({gameObject.name}) is trying to execute itself!");
                return;
            }

            CurrentExecutable.OnCompleted += OnExecutionCompleted;

            if (ThrowStraightStateInTweens)
            {
                CurrentExecutable.Execute();
            }
            else
            {
                CurrentExecutable.Execute();
            }
        }

        [ContextMenu("Stop")]
        public override void Stop()
        {
            base.Stop();

            if (CurrentExecutable)
            {
                CurrentExecutable.OnCompleted -= OnExecutionCompleted;
                CurrentExecutable = null;
            }

            IsPlaying = false;
        }
    }


}
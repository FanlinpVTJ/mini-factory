using System.Linq;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TweenComponents.Base
{
    public class TweenGroupExecute : Executable
    {
        [Header("Settings")]
        [SerializeField] public bool GetTweensOnAwake;
        [SerializeField] public bool FindMaxDurationTweenOnExecute;
        [Space]
        [SerializeField] private List<TweenBase> _tweensToExecute;

        //used to invoke OnComplete event when it completed
        private TweenBase _maxDurationTween;

        private void Awake()
        {
            if (_tweensToExecute == null || _tweensToExecute.Count == 0 || GetTweensOnAwake)
            {
                GetTweensFromChilds();
            }

            GetMaxDurationTween();
        }

        private void OnDestroy()
        {
            if (_maxDurationTween)
            {
                _maxDurationTween.OnCompleted -= OnExecutionCompleted;
            }
        }

        public override void Execute(bool isStraight = true)
        {
            if (!_maxDurationTween) return;

            if (FindMaxDurationTweenOnExecute)
            {
                GetMaxDurationTween();
            }

            _maxDurationTween.OnCompleted += OnExecutionCompleted;

            foreach (var tween in _tweensToExecute)
            {
                tween.Execute(isStraight);
            }

            base.Execute(isStraight);
        }

        public override void Stop()
        {
            base.Stop();

            foreach (var tween in _tweensToExecute)
            {
                tween.Stop();
            }
        }

        [ContextMenu("Get Tweens From Childs")]
        private void GetTweensFromChilds()
        {
            if (_tweensToExecute == null)
            {
                _tweensToExecute = new List<TweenBase>();
            }

            var tweens = GetComponentsInChildren<TweenBase>().Where(tween => !_tweensToExecute.Contains(tween));

            //remove null or missing elements
            _tweensToExecute = _tweensToExecute.Where(tween => tween).ToList();

            _tweensToExecute.AddRange(tweens);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(this);
            }
#endif
        }

        private void GetMaxDurationTween()
        {
            if (_tweensToExecute == null || _tweensToExecute.Count == 0) return;

            _maxDurationTween = _tweensToExecute.OrderByDescending(tween => tween.Duration).FirstOrDefault();

            if (_maxDurationTween)
            {
                _maxDurationTween.OnCompleted += OnExecutionCompleted;
            }
        }

        protected internal override void OnExecutionCompleted()
        {
            _maxDurationTween.OnCompleted -= OnExecutionCompleted;

            base.OnExecutionCompleted();
        }

#if UNITY_EDITOR

        [ContextMenu("Stop")]
        private void StopFromMenu()
        {
            Stop();
        }

#endif
    }
}


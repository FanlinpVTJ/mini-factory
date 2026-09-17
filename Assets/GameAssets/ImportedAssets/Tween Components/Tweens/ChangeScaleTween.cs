using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class ChangeScaleTween : TweenBase
    {
        public Vector3 StartScale;

        [Space]
        public bool SetCurrentScaleAsFinishValue;
        public Vector3 FinishScale;

        [Space]
        public Transform TransformToChange;

        private Vector3 _scaleToExecute;

        protected override void Awake()
        {
            if (!TransformToChange)
            {
                TransformToChange = transform;
            }

            if(SetCurrentScaleAsFinishValue)
            {
                FinishScale = TransformToChange.localScale;
            }

            base.Awake();
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && TransformToChange;
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            StartScale = TransformToChange.localScale;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            if (straight)
            {
                _scaleToExecute = FinishScale;
            }
            else
            {
                _scaleToExecute = StartScale;
            }

            _tween = TransformToChange.DOScale(_scaleToExecute, Duration).ApplyBaseSettings(this, TransformToChange.gameObject);
        }

        public override void ResetValue(bool applyStartValue = true)
        {
            if (applyStartValue)
            {
                TransformToChange.localScale = StartScale;
            }
            else
            {
                TransformToChange.localScale = FinishScale;
            }
        }
    }
}

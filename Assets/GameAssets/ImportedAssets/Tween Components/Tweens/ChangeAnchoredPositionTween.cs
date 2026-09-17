using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class ChangeAnchoredPositionTween : TweenBase
    {
        public Vector3 StartPosition;

        [Space]
        public bool SetCurrentPositionAsFinishValue;
        public Vector3 FinishPosition;

        [Space]
        public RectTransform TransformToChange;

        private Vector3 _positionToExecute;

        protected override void Awake()
        {
            if (!TransformToChange)
            {
                TransformToChange = transform as RectTransform;
            }

            base.Awake();
        }

        protected override void CheckValueOverrideOnAwake()
        {
            if (SetCurrentPositionAsFinishValue)
            {
                FinishPosition = TransformToChange.anchoredPosition;
            }

            base.CheckValueOverrideOnAwake();
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            StartPosition = TransformToChange.anchoredPosition;
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && TransformToChange;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            if (ResetValueOnExecute)
            {
                ResetValue(straight);
            }

            if (straight)
            {
                _positionToExecute = FinishPosition;
            }
            else
            {
                _positionToExecute = StartPosition;
            }

            _tween = TransformToChange.DOAnchorPos(_positionToExecute, Duration).ApplyBaseSettings(this, TransformToChange.gameObject);

        }

        public override void ResetValue(bool applyStartValue = true)
        {
            if (applyStartValue)
            {
                TransformToChange.anchoredPosition = StartPosition;
            }
            else
            {
                TransformToChange.anchoredPosition = FinishPosition;
            }
        }
    }
}
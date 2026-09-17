using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;

namespace Cryptogram
{
    public class ChangeRectSizeTween : TweenBase
    {
        public Vector2 StartSize;

        [Space]
        public bool SetCurrentSizeAsFinishValue;
        public Vector2 FinishSize;

        [Space]
        public RectTransform TransformToChange;

        private Vector2 _sizeToExecute;

        protected override void Awake()
        {
            if (!TransformToChange)
            {
                TransformToChange = transform as RectTransform;
            }

            if (SetCurrentSizeAsFinishValue)
            {
                FinishSize = TransformToChange.sizeDelta;
            }

            base.Awake();
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && TransformToChange;
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            StartSize = TransformToChange.sizeDelta;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;
            base.Execute(straight);
            _sizeToExecute = straight ? FinishSize : StartSize;
            _tween = TransformToChange.DOSizeDelta(_sizeToExecute, Duration).ApplyBaseSettings(this, TransformToChange.gameObject);
        }

        public override void ResetValue(bool applyStartValue = true)
        {
            TransformToChange.sizeDelta = applyStartValue ? StartSize : FinishSize;
        }
    }
}

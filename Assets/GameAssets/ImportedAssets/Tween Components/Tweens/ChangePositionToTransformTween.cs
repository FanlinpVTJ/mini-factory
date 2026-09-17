using UnityEngine;
using DG.Tweening;
using TweenComponents.Base;

namespace TweenComponents
{
    public class ChangePositionToTransformTween : TweenBase
    {
        [Space]
        public Transform TransformToChange;

        [Space]
        public Transform TargetPosition;

        protected override void Awake()
        {
            if (!TransformToChange)
            {
                TransformToChange = transform;
            }

            base.Awake();
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && TargetPosition && TransformToChange;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            _tween = TransformToChange.DOBlendableMoveBy(TargetPosition.position - TransformToChange.position, Duration)
                .ApplyBaseSettings(this, TransformToChange.gameObject);
        }

        protected override void ApplyCurrentValueAsStartValue() { }
        public override void ResetValue(bool applyStartValue = true) { }
    }
}
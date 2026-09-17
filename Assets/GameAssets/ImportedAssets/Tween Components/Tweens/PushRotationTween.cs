using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class PushRotationTween : TweenBase
    {
        public Vector3 PushDelta;

        [Space]
        public int BounceCount;
        [Range(0f, 1f)] public float Elasticity;

        [Space]
        public Transform TransformToChange;

        protected override void Awake()
        {
            if (!TransformToChange)
            {
                TransformToChange = transform;
            }

            base.Awake();
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            _tween = TransformToChange.DOPunchRotation(PushDelta, Duration, BounceCount, Elasticity)
                .ApplyBaseSettings(this, TransformToChange.gameObject);
        }

        protected override void ApplyCurrentValueAsStartValue() { }

        public override void ResetValue(bool applyStartValue = true) { }
    }
}
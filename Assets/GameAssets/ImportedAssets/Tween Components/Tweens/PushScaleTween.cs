using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class PushScaleTween : TweenBase
    {
        public Vector3 ScaleChange;
        public int BounceCount;
        [Range(0, 1)] public float Elasticity;

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

            _tween = TransformToChange.DOPunchScale(ScaleChange, Duration, BounceCount, Elasticity)
                .SetDelay(Delay)
                .SetLoops(LoopCount, LoopType)
                .SetUpdate(IgnoreTimeScale)
                .SetEase(EaseType)
                .SetLink(TransformToChange.gameObject)
                .SetAutoKill(true)
                .OnComplete(OnExecutionCompleted);
        }

        protected override void ApplyCurrentValueAsStartValue() { }

        public override void ResetValue(bool applyStartValue = true) { }
    }
}
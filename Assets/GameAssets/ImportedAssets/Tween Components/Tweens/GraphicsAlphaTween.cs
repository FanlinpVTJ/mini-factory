using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TweenComponents
{
    public class GraphicsAlphaTween : TweenBase
    {
        [Min(0)]
        public float StartAlpha = 0;

        [Space]
        public bool SetCurrentColorAsFinishValue;
        [Min(0)]
        public float FinishAlpha = 1;

        [Space]
        public Graphic TargetGraphic;

        private float _targetAlpha;

        protected override void Awake()
        {
            if (!TargetGraphic)
            {
                TargetGraphic = GetComponent<Graphic>();
            }

            if (TargetGraphic)
            {
                CheckValueOverrideOnAwake();
            }
            else
            {
                LogError("There`s no Graphic");
            }
        }

        protected override void CheckValueOverrideOnAwake()
        {
            if (SetCurrentColorAsFinishValue)
            {
                FinishAlpha = TargetGraphic.color.a;
            }

            base.CheckValueOverrideOnAwake();
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            StartAlpha = TargetGraphic.color.a;
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && TargetGraphic;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            _targetAlpha = TargetGraphic.color.a;

            if (straight)
            {
                _targetAlpha = FinishAlpha;
            }
            else
            {
                _targetAlpha = StartAlpha;
            }

            _tween = TargetGraphic.DOFade(_targetAlpha, Duration).ApplyBaseSettings(this, TargetGraphic.gameObject);
        }

        public override void ResetValue(bool applyStartValue = true)
        {
            var color = TargetGraphic.color;

            if (applyStartValue)
            {
                color.a = StartAlpha;
            }
            else
            {
                color.a = FinishAlpha;
            }
            
            TargetGraphic.color = color;
        }
    }
}


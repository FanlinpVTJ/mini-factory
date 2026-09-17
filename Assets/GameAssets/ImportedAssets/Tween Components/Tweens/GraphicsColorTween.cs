using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TweenComponents
{
    public class GraphicsColorTween : TweenBase
    {
        public Color StartColor = Color.white;

        [Space]
        public bool SetCurrentColorAsFinishValue;
        public Color FinishColor = Color.white;

        [Space]
        public Graphic TargetGraphic;

        private Color _targetColor;

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
                FinishColor = TargetGraphic.color;
            }

            base.CheckValueOverrideOnAwake();
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            StartColor = TargetGraphic.color;
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && TargetGraphic;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            _targetColor = TargetGraphic.color;

            if (straight)
            {
                _targetColor = FinishColor;
            }
            else
            {
                _targetColor = StartColor;
            }

            _tween = TargetGraphic.DOColor(_targetColor, Duration).ApplyBaseSettings(this, TargetGraphic.gameObject);
        }

        public override void ResetValue(bool applyStartValue = true)
        {
            if (applyStartValue)
            {
                TargetGraphic.color = StartColor;
            }
            else
            {
                TargetGraphic.color = FinishColor;
            }
        }
    }
}


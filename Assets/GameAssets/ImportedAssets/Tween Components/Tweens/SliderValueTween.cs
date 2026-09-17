using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TweenComponents
{
    public class SliderValueTween : TweenBase
    {
        public float StartValue;

        [Space]
        public bool SetCurrentAsFinishValue;
        public float FinishValue;

        [Space]
        public Slider Slider;

        private float _targetValue;

        protected override void Awake()
        {
            if (!Slider)
            {
                Slider = GetComponent<Slider>();
            }

            if (Slider)
            {
                base.Awake();
            }
            else
            {
                LogError("There`s no Slider");
            }
        }

        protected override void CheckValueOverrideOnAwake()
        {
            if (SetCurrentAsFinishValue)
            {
                FinishValue = Slider.value;
            }

            base.CheckValueOverrideOnAwake();
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            StartValue = Slider.value;
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && Slider;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            _targetValue = straight ? FinishValue : StartValue;

            _tween = Slider.DOValue(_targetValue, Duration).ApplyBaseSettings(this, Slider.gameObject);
        }

        public override void ResetValue(bool applyStartValue = true)
        {
            if (applyStartValue)
            {
                Slider.value = StartValue;
            }
            else
            {
                Slider.value = FinishValue;
            }
        }
    }
}
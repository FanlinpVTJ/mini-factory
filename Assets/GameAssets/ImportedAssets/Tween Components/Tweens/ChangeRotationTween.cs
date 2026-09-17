using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class ChangeRotationTween : TweenBase
    {
        public bool ChangeLocalRotation;
        public Vector3 StartValue;

        [Space]
        public bool SetCurrentRotationAsFinishValue;
        public Vector3 FinishValue;

        [Space]
        public Transform TransformToChange;

        private Vector3 _target;

        protected override void Awake()
        {
            if (!TransformToChange)
            {
                TransformToChange = transform;
            }

            base.Awake();
        }

        protected override void CheckValueOverrideOnAwake()
        {
            if (SetCurrentRotationAsFinishValue)
            {
                if (ChangeLocalRotation)
                    FinishValue = TransformToChange.localRotation.eulerAngles;
                else
                    FinishValue = TransformToChange.rotation.eulerAngles;
            }

            base.CheckValueOverrideOnAwake();
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            if(ChangeLocalRotation)
                StartValue = TransformToChange.localRotation.eulerAngles;
            else
                StartValue = TransformToChange.rotation.eulerAngles;
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && TransformToChange;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            _target = straight ? FinishValue : StartValue;

            if (ChangeLocalRotation)
            {
                _tween = TransformToChange.DOLocalRotate(_target, Duration)
                    .ApplyBaseSettings(this, TransformToChange.gameObject);
            }
            else
            {
                _tween = TransformToChange.DORotate(_target, Duration)
                    .ApplyBaseSettings(this, TransformToChange.gameObject);
            }

        }

        public override void ResetValue(bool applyStartValue = true)
        {
            if (applyStartValue)
            {
                if(ChangeLocalRotation)
                    TransformToChange.localRotation = Quaternion.Euler(StartValue);
                else
                    TransformToChange.rotation = Quaternion.Euler(StartValue);
            }
            else
            {
                if(ChangeLocalRotation)
                    TransformToChange.localRotation = Quaternion.Euler(FinishValue);
                else
                    TransformToChange.rotation = Quaternion.Euler(FinishValue);
            }
        }
    }
}
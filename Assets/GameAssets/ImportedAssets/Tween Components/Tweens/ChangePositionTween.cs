using DG.Tweening;
using System;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class ChangePositionTween : TweenBase
    {
        public bool ChangeLocalPosition;
        public Vector3 StartPosition;

        [Space]
        public bool SetCurrentPositionAsFinishValue;
        public Vector3 FinishPosition;

        [Space]
        public Transform TransformToChange;


        private Vector3 _positionToExecute;

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
            if (SetCurrentPositionAsFinishValue)
            {
                FinishPosition = TransformToChange.localPosition;
            }

            base.CheckValueOverrideOnAwake();
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            StartPosition = TransformToChange.localPosition;
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

            if (ChangeLocalPosition)
            {
                _tween = TransformToChange.DOLocalMove(_positionToExecute, Duration).ApplyBaseSettings(this, TransformToChange.gameObject);
            }
            else
            {
                _tween = TransformToChange.DOMove(_positionToExecute, Duration).ApplyBaseSettings(this, TransformToChange.gameObject);
            }
        }

        public override void ResetValue(bool applyStartValue = true)
        {
            if (applyStartValue)
            {
                if (ChangeLocalPosition)
                {
                    TransformToChange.localPosition = StartPosition;
                }
                else
                {
                    TransformToChange.position = StartPosition;
                }
            }
            else
            {
                if (ChangeLocalPosition)
                {
                    TransformToChange.localPosition = FinishPosition;
                }
                else
                {
                    TransformToChange.position = FinishPosition;
                }
            }
        }
    }
}
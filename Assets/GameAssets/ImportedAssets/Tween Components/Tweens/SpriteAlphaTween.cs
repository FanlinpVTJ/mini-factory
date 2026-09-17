using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class SpriteAlphaTween : TweenBase
    {
        public float StartAlpha;

        [Space]
        public bool SetCurrentAlphaAsFinishValue;
        public float EndAlpha;

        [Space]
        public SpriteRenderer TargetSprite;

        private Color _targetColor;

        protected override void Awake()
        {
            if (!TargetSprite)
            {
                TargetSprite = GetComponent<SpriteRenderer>();
            }

            if (TargetSprite)
            {
                CheckValueOverrideOnAwake();
            }
            else
            {
                Debug.LogError($"There`s no Image for tween on {gameObject.name}!");
            }
        }

        protected override void CheckValueOverrideOnAwake()
        {
            if (SetCurrentAlphaAsFinishValue)
            {
                EndAlpha = TargetSprite.color.a;
            }

            base.CheckValueOverrideOnAwake();
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            StartAlpha = TargetSprite.color.a;
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && TargetSprite;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            _targetColor = TargetSprite.color;

            if (straight)
            {
                _targetColor.a = EndAlpha;
            }
            else
            {
                _targetColor.a = StartAlpha;
            }

            _tween = TargetSprite.DOColor(_targetColor, Duration).ApplyBaseSettings(this, TargetSprite.gameObject);
        }

        public override void ResetValue(bool applyStartValue = true)
        {
            var color = TargetSprite.color;

            if (applyStartValue)
            {
                color.a = StartAlpha;
            }
            else
            {
                color.a = EndAlpha;
            }

            TargetSprite.color = color;
        }
    }
}
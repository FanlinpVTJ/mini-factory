using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class RendererColorTween : TweenBase
    {
        public Color StartColor = Color.white;

        [Space]
        public bool SetCurrentAlphaAsFinishValue;
        public Color FinishColor = Color.white;

        [Space]
        public Renderer TargetRenderer;
        public int MaterialIndex = 0;

        private Material _targetMaterial;        
        private Color _targetColor;

        protected override void Awake()
        {
            if (!TargetRenderer)
            {
                TargetRenderer = GetComponent<Renderer>();
            }

            if (TargetRenderer)
            {
                if(MaterialIndex < TargetRenderer.materials.Length)
                    _targetMaterial = TargetRenderer.materials[MaterialIndex];

                if (_targetMaterial)
                {
                    CheckValueOverrideOnAwake();
                    return;
                }
            }

            LogError("There`s no Renderer or Material");
        }

        protected override void CheckValueOverrideOnAwake()
        {
            if (SetCurrentAlphaAsFinishValue)
            {
                FinishColor = _targetMaterial.color;
            }

            base.CheckValueOverrideOnAwake();
        }

        protected override void ApplyCurrentValueAsStartValue()
        {
            StartColor = _targetMaterial.color;
        }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && _targetMaterial;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            _targetColor = _targetMaterial.color;

            if (straight)
            {
                _targetColor = FinishColor;
            }
            else
            {
                _targetColor = StartColor;
            }

            _tween = _targetMaterial.DOColor(_targetColor, Duration).ApplyBaseSettings(this, TargetRenderer.gameObject);
        }

        public override void ResetValue(bool applyStartValue = true)
        {
            if (applyStartValue)
            {
                _targetMaterial.color = StartColor;
            }
            else
            {
                _targetMaterial.color = FinishColor;
            }
        }
    }
}


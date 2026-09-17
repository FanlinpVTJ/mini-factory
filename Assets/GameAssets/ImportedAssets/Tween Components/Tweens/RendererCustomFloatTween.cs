using DG.Tweening;
using TweenComponents.Base;
using UnityEngine;

namespace TweenComponents
{
    public class RendererCustomFloatTween : TweenBase
    {
        public float StartValue;

        [Space]
        public float FinishValue;

        [Space]
        public Renderer TargetRenderer;
        public int MaterialIndex = 0;
        public string ValueName;

        private Material _targetMaterial;
        private float _targetValue;

        protected override void Awake()
        {
            if (!TargetRenderer)
            {
                TargetRenderer = GetComponent<Renderer>();
            }

            if (TargetRenderer)
            {
                if (MaterialIndex < TargetRenderer.materials.Length)
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
            base.CheckValueOverrideOnAwake();
        }

        protected override void ApplyCurrentValueAsStartValue() { }

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && _targetMaterial;
        }

        public override void Execute(bool straight = true)
        {
            if (!CanBeExecuted()) return;

            base.Execute(straight);

            if (straight)
            {
                _targetValue = FinishValue;
            }
            else
            {
                _targetValue = StartValue;
            }            

            _tween = DOTween.To(() => _targetMaterial.GetFloat(ValueName), (value) => _targetMaterial.SetFloat(ValueName, value), _targetValue, Duration)
                .ApplyBaseSettings(this, TargetRenderer.gameObject);
        }

        public override void ResetValue(bool applyStartValue = true)
        {
            if (applyStartValue)
            {
                _targetMaterial.SetFloat(ValueName, StartValue);
            }
            else
            {
                _targetMaterial.SetFloat(ValueName, FinishValue);
            }
        }
    }
}


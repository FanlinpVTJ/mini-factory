using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ValueSystem.Misc;
using Zenject;

namespace ValueSystem.UI
{
    public class BaseValueView : MonoBehaviour
    {
        public float CurrentValue => _valueHandler.Value;
        public float CurrentViewValue => _value;

        [SerializeField]
        protected ValueData _valueData;
        [SerializeField]
        protected bool _formatText = true;

        [Space]        
        [SerializeField]
        protected Image _valueIcon;
        [SerializeField]
        protected TMP_Text _valueText;

        [Inject]
        protected IValueSystem _valueSystem;

        protected ValueHandler _valueHandler;

        protected float _value;

        protected virtual void Start()
        {
            if (_valueData && _valueIcon)
                _valueIcon.sprite = _valueData.Icon;
        }

        protected virtual void OnEnable()
        {
            if (!_valueData) return;
            if (!_valueText) return;

            if (_valueHandler == null)
                _valueHandler = _valueSystem.GetValueHandler(_valueData);

            UpdateValue();
        }

        protected virtual void UpdateValue()
        {
            SetValueCountView(_valueHandler.Value);
        }

        public void SetValueCountView(float value)
        {
            _value = value;

            if (_formatText)
                _valueText.text = TextFormatter.FormatNumber(value);
            else
                _valueText.text = value.ToString();
        }

        protected virtual void OnDisable() { }
    }
}



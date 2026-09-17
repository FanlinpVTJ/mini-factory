using TweenComponents.Base;
using UnityEngine;

namespace ValueSystem.UI
{
    public class ValueView : BaseValueView
	{
		[Space]
		[SerializeField]
		private TweenBase _animation;

		protected override void Start()
		{
			if (_valueData && _valueIcon)
				_valueIcon.sprite = _valueData.Icon;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (_valueHandler != null)
				_valueHandler.OnValueChanged += UpdateValue;

		}

		protected override void OnDisable()
		{
			if (_valueHandler != null)
				_valueHandler.OnValueChanged -= UpdateValue;
		}

		protected override void UpdateValue()
		{
			base.UpdateValue();

			if (_animation)
				_animation.Execute();
		}
	}
}



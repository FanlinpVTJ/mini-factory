using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ValueSystem.UI
{
	[RequireComponent(typeof(Button))]
	public class AddValueButton : MonoBehaviour
	{
		[SerializeField]
		private ValueData _valueData;
		[SerializeField]
		private float _countAddValue;

		[Inject]
		private IValueSystem _valueSystem;

		private ValueHandler _valueHandler;
		private Button _button;

		private void Awake()
		{
#if !UNITY_EDITOR
			gameObject.SetActive(false);
#endif
			_button = GetComponent<Button>();
			_button.onClick.AddListener(Click);
			_valueHandler = _valueSystem.GetValueHandler(_valueData);
		}

		private void OnDestroy()
		{
			_button.onClick.RemoveListener(Click);
		}

		private void Click()
		{
			_valueHandler.Add(_countAddValue);
		}
	}
}
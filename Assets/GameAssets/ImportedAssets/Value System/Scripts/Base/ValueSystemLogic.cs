using ValueSystem.Save;
using System.Collections.Generic;

namespace ValueSystem.Base
{
	public class ValueSystemLogic : IValueSystem
	{
		public IReadOnlyDictionary<string, ValueHandler> ValueHandler => _valueHandlers;

		private readonly ValueData[] _datas;

		private ValuesSave _save;

		private Dictionary<string, ValueHandler> _valueHandlers;

		public ValueSystemLogic(ValueData[] datas, ValuesSave save = null)
		{
			_valueHandlers = new();
			_datas = datas;

			if (save != null)
				Setup(save);
		}

		public void Setup(ValuesSave save)
		{
			_save = save;

			foreach (var data in _datas)
			{
				if (!data) continue;

				_valueHandlers.Add(data.Id, new(_save, data));
			}
		}

		public ValueHandler GetValueHandler(string valueID)
		{
			if (_valueHandlers.ContainsKey(valueID))
				return _valueHandlers[valueID];

			return null;
		}
		public ValueHandler GetValueHandler(ValueData data)
		{
			if (!data)
			{
				UnityEngine.Debug.LogError("No value data, can`t get value!");
				return null;
			}

			return GetValueHandler(data.Id);
		}

		public bool TrySubtract(string valueID, float amount, bool forced = false)
		{
			if (_valueHandlers.ContainsKey(valueID))
				return _valueHandlers[valueID].TrySubtract(amount, forced);

			return false;
		}
		public bool TrySubtract(ValueData data, float amount, bool forced = false)
		{
			if (!data)
			{
				UnityEngine.Debug.LogError("No value data, can`t spend value!");
				return false;
			}

			return TrySubtract(data.Id, amount, forced);
		}

		public bool CanSubtract(string valueID, float amount)
		{
			if (_valueHandlers.ContainsKey(valueID))
				return _valueHandlers[valueID].CanSubtract(amount);

			return false;
		}
		public bool CanSubtract(ValueData data, float amount)
		{
			if (!data)
			{
				UnityEngine.Debug.LogError("No value data, can`t check if can spend value!");
				return false;
			}

			return CanSubtract(data.Id, amount);
		}

		public bool Add(string valueID, float amount)
		{
			if (_valueHandlers.ContainsKey(valueID))
				return _valueHandlers[valueID].Change(amount);

			return false;
		}
		public bool Add(ValueData data, float amount)
		{
			if (!data)
			{
				UnityEngine.Debug.LogError("No value data, can`t add value!");
				return false;
			}

			return Add(data.Id, amount);
		}

		public bool Change(string valueID, float amount, bool forced = false)
		{
			if (_valueHandlers.ContainsKey(valueID))
				return _valueHandlers[valueID].Change(amount, forced);

			return false;
		}
		public bool Change(ValueData data, float amount, bool forced)
		{
			if (!data)
			{
				UnityEngine.Debug.LogError("No value data, can`t change value!");
				return false;
			}

			return Change(data.Id, amount, forced);
		}

		public void SetValue(string id, float value)
		{
			if (_valueHandlers.ContainsKey(id))
				_valueHandlers[id].Set(value);
		}
		public void SetValue(ValueData data, float value)
		{
			if (!data)
			{
				UnityEngine.Debug.LogError("No value data, can`t set value!");
				return;
			}

			SetValue(data.Id, value);
		}

		public float GetValue(string valueId)
		{
			if (_valueHandlers.ContainsKey(valueId))
				return _valueHandlers[valueId].Value;

			return 0;
		}
		public float GetValue(ValueData data)
		{
			if (!data)
			{
				UnityEngine.Debug.LogError("No value data, can`t get value!");
				return 0;
			}

			return GetValue(data.Id);
		}
	}
}


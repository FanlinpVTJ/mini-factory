using System;
using UnityEngine.Rendering;

namespace ValueSystem.Save
{
	[System.Serializable]
	public class ValuesSave
    {
		public CurrencyAmounts ValueAmounts = new CurrencyAmounts();

		private Action _saveAction;

        [Serializable]
        public sealed class CurrencyAmounts : SerializedDictionary<string, float>
        {
        }

        public ValuesSave(Action saveAction)
		{
			_saveAction = saveAction;
		}

        public void SetSaveAction(Action saveAction)
        {
            _saveAction = saveAction;
        }

		public float GetValue(string id, float defaultAmmount)
		{
			if (!ValueAmounts.ContainsKey(id))
			{
				ValueAmounts.Add(id, defaultAmmount);
			}

			return ValueAmounts[id];
		}

		public float GetValue(ValueData data)
		{
			return GetValue(data.Id, data.DefaulCount);
		}

		public void UpdateValue(string id, float amount)
		{
			if (!ValueAmounts.ContainsKey(id))
				ValueAmounts.Add(id, amount);
			else
				ValueAmounts[id] = amount;

			Save();
		}

		public void UpdateValue(ValueData data, float amount)
		{
			UpdateValue(data.Id, amount);
		}

		private void Save()
		{
			_saveAction?.Invoke();
        }
	}
}


using ValueSystem.Save;
using System;

namespace ValueSystem
{
    public class ValueHandler
    {
        public event Action OnValueChanged;
        public event Action<float> OnAdd;
        public event Action<float> OnSubtract;

        public readonly ValueData Data;

        public float Value
        {
            get => _value;
            private set
            {
                _value = value;
                _save.UpdateValue(Data.Id, _value);
                OnValueChanged?.Invoke();
            }
        }

        public string Id => Data.Id;
        public bool IsInfinite => Data.IsInfinite;
        public float MaxCount => Data.MaxCount;
        public float MinCount => Data.MinCount;
        public bool IsFullFilled => IsInfinite ? false : Value >= MaxCount;

        private readonly ValuesSave _save;

        private float _value;

        public ValueHandler(ValuesSave save, ValueData data)
        {
            _save = save;

            Data = data;
            Value = _save.GetValue(Id, data.DefaulCount);
        }

        public bool CanSubtract(float amount)
        {
            return Value - amount >= MinCount;
        }

        public bool TrySubtract(float amount, bool forced = false)
        {
            if (CanSubtract(amount) || forced)
            {
                Value -= amount;

                if (Value < MinCount)
                    Value = MinCount;

                OnSubtract?.Invoke(amount);
                return true;
            }

            return false;
        }

        public bool Add(float amount)
        {
            if (!IsInfinite)
            {
                if (IsFullFilled) return false;

                if (Value + amount > MaxCount)
                {
                    amount = MaxCount - Value;
                }
            }

            Value += amount;
            OnAdd?.Invoke(amount);

            return true;
        }

        public bool Change(float amount, bool forced = false)
        {
            if (amount >= 0)
                return Add(amount);
            else
                return TrySubtract(-amount);
        }

        public void Set(float value)
        {
            Value = value;
        }

        public void Save()
        {
            _save.UpdateValue(Id, _value);
        }
    }
}

